using FindTheIdol.Components.Altar;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.World;
using FindTheIdol.MonoBehaviours;
using FindTheIdol.MonoBehaviours.World;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

namespace FindTheIdol.MonoBehaviours.Altar
{
    [RequiresEntityConversion]
    public class AltarAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        [UnityEngine.Header("(red box)")]
        [UnityEngine.SerializeField]
        private float3 scale;

        [UnityEngine.Header("(green box)")]
        [UnityEngine.SerializeField]
        private float3 colliderScale;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject altarGameObject;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            WorldBounds worldBounds = WorldBoundsStaticAccessor.WorldBounds;
            
            UnityEngine.Mesh altarMesh = altarGameObject.GetComponent<UnityEngine.MeshFilter>().mesh;
            UnityEngine.Material altarMaterial = altarGameObject.GetComponent<UnityEngine.MeshRenderer>().material;
            UnityEngine.Transform altarTransform = altarGameObject.GetComponent<UnityEngine.Transform>();

            using(NativeArray<Entity> altar = dstManager.CreateEntity(dstManager.CreateAltarArchetype(), 1, Allocator.Temp))
            {
                dstManager.SetComponentData(altar[0], new Translation 
                { 
                    Value = new float3
                    (
                        UnityEngine.Random.Range(worldBounds.XZGridWorldSpaceMin.x, worldBounds.XZGridWorldSpaceMax.x),
                        0f,
                        UnityEngine.Random.Range(worldBounds.XZGridWorldSpaceMin.y, worldBounds.XZGridWorldSpaceMax.y)
                    )
                });

                dstManager.SetComponentData(altar[0], new NonUniformScale
                {
                    Value = altarMesh.bounds.size
                });
                
                dstManager.SetComponentData(altar[0], new Rotation
                {
                    Value = altarTransform.rotation
                });

                dstManager.SetComponentData(altar[0], new PhysicsCollider
                {
                    Value = BoxCollider.Create
                    (
                        CollisionGeomeotrySingleton.Instance.CreateOrGetBoxGeometry(this.colliderScale), 
                        CollisionFilterSingleton.Instance.CollidesWithPlayerFilter
                    )
                });

                dstManager.SetSharedComponentData<RenderMesh>(altar[0], new RenderMesh 
                { 
                    mesh = altarMesh,
                    material = altarMaterial
                });
            } 

            DestroyImmediate(this.altarGameObject);  
        }

        #if UNITY_EDITOR
        
        //Provide this gizmo so you can see the ecs collider sizes in unity editor
        private void OnDrawGizmosSelected()
        {
            UnityEngine.Gizmos.color = new UnityEngine.Color(1, 0, 0, 0.5f);
            UnityEngine.Gizmos.DrawCube(this.scale / 2, this.scale);
            UnityEngine.Gizmos.color = new UnityEngine.Color(0, 1, 0, 0.5f);
            UnityEngine.Gizmos.DrawCube(this.colliderScale / 2, this.colliderScale);
        }

        #endif
    }
}
