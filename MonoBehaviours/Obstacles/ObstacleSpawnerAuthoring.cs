using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Obstacles;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

namespace FindTheIdol.MonoBehaviours.Obstacles
{
    [RequiresEntityConversion]
    public class ObstacleSpawnerAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        private UnityEngine.GameObject[] obstacleGameObjects;

        private UnityEngine.Mesh[] meshesOfGameObjects;

        private UnityEngine.Material[] materialsOfGameObjects;

        private IList<ObstacleDTO> obstacleDTOs;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject obstacleHolder;

        private void Awake() 
        {
            //Only select the children of the gameobject, but have to go the route over transforms, because there does not exist a GetChildGameObjects :(
            this.obstacleDTOs = new List<ObstacleDTO>();
            //This is kind of akward -> have to find the child gameobjects by collecting the childrens transform gameobject
            this.obstacleGameObjects = this.obstacleHolder.GetComponentsInChildren<UnityEngine.Transform>().Where(trans => trans.childCount == 0).Select(obj => obj.gameObject).ToArray();

            foreach(UnityEngine.GameObject obstacle in this.obstacleGameObjects)
            {
                UnityEngine.BoxCollider overrideCollider = default;
                if(obstacle.TryGetComponent(out overrideCollider))
                {
                    this.obstacleDTOs.Add(new ObstacleDTO
                    {
                        Transform = obstacle.GetComponent<UnityEngine.Transform>(),
                        Mesh = obstacle.GetComponent<UnityEngine.MeshFilter>().mesh,
                        Materials = obstacle.GetComponent<UnityEngine.MeshRenderer>().materials,
                        WorldSpaceBounds = obstacle.GetComponent<UnityEngine.MeshRenderer>().bounds,
                        OverrideBoxCollider = overrideCollider
                    });
                }
                else
                {
                    this.obstacleDTOs.Add(new ObstacleDTO
                    {
                        Transform = obstacle.GetComponent<UnityEngine.Transform>(),
                        Mesh = obstacle.GetComponent<UnityEngine.MeshFilter>().mesh,
                        Materials = obstacle.GetComponent<UnityEngine.MeshRenderer>().materials,
                        WorldSpaceBounds = obstacle.GetComponent<UnityEngine.MeshRenderer>().bounds,
                        //Have to check manually for this null
                        OverrideBoxCollider = null
                    });
                }
            }
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            using(NativeArray<Entity> obstacleEntities = dstManager.CreateEntity(dstManager.CreateObstacleArchetype(), this.obstacleDTOs.Count, Allocator.Temp))
            {
                for(int i = 0; i < obstacleDTOs.Count; ++i)
                {
                    dstManager.SetComponentData(obstacleEntities[i], new Translation
                    {
                        Value = this.obstacleDTOs[i].Transform.position
                    });

                    dstManager.SetComponentData(obstacleEntities[i], new Rotation
                    {
                        Value = this.obstacleDTOs[i].Transform.rotation
                    });

                    dstManager.SetComponentData(obstacleEntities[i], new NonUniformScale
                    {
                        Value = this.obstacleDTOs[i].Transform.lossyScale
                    });

                    dstManager.SetComponentData(obstacleEntities[i], new RenderBounds
                    {
                        Value = new AABB
                        {
                           Center = this.obstacleDTOs[i].Mesh.bounds.center,
                           Extents = this.obstacleDTOs[i].Mesh.bounds.extents
                        } 
                    });

                    if(this.obstacleDTOs[i].OverrideBoxCollider == null)
                    {
                        dstManager.SetComponentData(obstacleEntities[i], new PhysicsCollider
                        {
                            Value = BoxCollider.Create
                            (
                                CollisionGeomeotrySingleton.Instance.CreateOrGetBoxGeometry(this.obstacleDTOs[i].WorldSpaceBounds.size), 
                                CollisionFilterSingleton.Instance.BelongsToNonTraversableFilter
                            )
                        });
                    }
                    else
                    {
                        dstManager.SetComponentData(obstacleEntities[i], new PhysicsCollider
                        {
                            Value = BoxCollider.Create
                            (
                                CollisionGeomeotrySingleton.Instance.CreateOrGetBoxGeometry(this.obstacleDTOs[i].OverrideBoxCollider.size, this.obstacleDTOs[i].OverrideBoxCollider.center), 
                                CollisionFilterSingleton.Instance.BelongsToNonTraversableFilter
                            )
                        });
                    }

                    dstManager.AddSharedComponentData(obstacleEntities[i], new RenderMesh 
                    { 
                        mesh = this.obstacleDTOs[i].Mesh,
                        //Currently just choose the first material, but later on it should be possible to have more than 1 material for meshes
                        material = this.obstacleDTOs[i].Materials[0]
                    });
                }
            }

            DestroyImmediate(obstacleHolder);
        }
    }
}
