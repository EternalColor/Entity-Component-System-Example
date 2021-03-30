using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Pathfinding;
using FindTheIdol.Components.World;
using FindTheIdol.Pathfinding;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FindTheIdol.MonoBehaviours.World
{
    [RequiresEntityConversion]
    public class WorldAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        [UnityEngine.SerializeField]
        private UnityEngine.GameObject Map;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            UnityEngine.MeshRenderer meshRenderer = this.Map.GetComponent<UnityEngine.MeshRenderer>();

            dstManager.AddComponentData(entity, new PhysicsCollider
            {
                Value = BoxCollider.Create
                (
                    CollisionGeomeotrySingleton.Instance.CreateOrGetBoxGeometry(meshRenderer.bounds.size), 
                    CollisionFilterSingleton.Instance.BelongsToNonTraversableFilter
                )
            });

            Entity worldBoundsEntity = dstManager.CreateEntity(dstManager.CreateWorldBoundsArchetype());

            WorldBounds worldBounds = WorldBoundsStaticAccessor.WorldBounds;

            dstManager.SetComponentData(worldBoundsEntity, worldBounds);

            using(NativeArray<Entity> pathNodes = dstManager.CreateEntity(dstManager.CreatePathnodeArchetype(), worldBounds.XZGridSize.x * worldBounds.XZGridSize.y, Allocator.Temp))
            {
                for(int x = 0; x < worldBounds.XZGridSize.x; ++x)
                {
                    for(int y = 0; y < worldBounds.XZGridSize.y; ++y)
                    {
                        int index = AStarUtility.CalculateIndex(x, y, worldBounds.XZGridSize.x);

                        dstManager.SetComponentData(pathNodes[index], new PathNode
                        { 
                            X = x, 
                            Y = y, 
                            Index = index,
                            GCost = int.MaxValue,
                            HCost = int.MaxValue,
                            IsTraversable = true, //Phyisics check in MonsterPathfindingGridSystem.cs but not in FindPath Method of AStar.cs saves performance!
                            PreviousNodeIndex = -1
                        }); 
                    }
                }
            }
        }
    }
}
