using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Monsters;
using FindTheIdol.Components.Pathfinding;
using FindTheIdol.Components.World;
using FindTheIdol.MonoBehaviours.World;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

namespace FindTheIdol.MonoBehaviours.Monsters
{
    [RequiresEntityConversion]
    public class MonsterSpawnerAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        [UnityEngine.SerializeField]
        private MonsterDTO[] spawnInWorldMonsters;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            WorldBounds worldBounds = WorldBoundsStaticAccessor.WorldBounds;

            int totalSpawnCount = this.spawnInWorldMonsters.Select(item => item.SpawnAmount).Sum();

            using(NativeArray<Entity> monsters = dstManager.CreateEntity(dstManager.CreateMonsterArchetype(), totalSpawnCount, Allocator.Temp))
            {
                 //The offset (spawn amount) per MonsterType can be different for every entry so we save it to a variable.
                int monsterOffset = 0;

                for(int i = 0; i < spawnInWorldMonsters.Length; ++i)
                {
                    RenderMesh renderMesh = new RenderMesh 
                    { 
                        mesh = this.spawnInWorldMonsters[i].Mesh,
                        material = this.spawnInWorldMonsters[i].Material
                    };

                    for(int j = 0; j < spawnInWorldMonsters[i].SpawnAmount; ++j)
                    {
                        dstManager.AddBuffer<PathCell>(monsters[j + monsterOffset]);

                        DynamicBuffer<DropItem> dropItems = dstManager.AddBuffer<DropItem>(monsters[j + monsterOffset]);
                        for(int k = 0; k < spawnInWorldMonsters[i].DropItems.Length; ++k)
                        {
                            dropItems.Add(new DropItem { ItemType = spawnInWorldMonsters[i].DropItems[k] });
                        }

                        dstManager.SetComponentData(monsters[j + monsterOffset], spawnInWorldMonsters[i].MonsterInfo);

                        dstManager.SetComponentData(monsters[j + monsterOffset], new PathFollow { PathIndex = -1 });

                        dstManager.SetComponentData(monsters[j + monsterOffset], new Translation 
                        { 
                            Value = new float3
                            (
                                UnityEngine.Random.Range(worldBounds.XZGridWorldSpaceMin.x, worldBounds.XZGridWorldSpaceMax.x),
                                3f,
                                UnityEngine.Random.Range(worldBounds.XZGridWorldSpaceMin.y, worldBounds.XZGridWorldSpaceMax.y)
                            )
                        });

                        dstManager.SetComponentData(monsters[j + monsterOffset], new NonUniformScale 
                        { 
                            Value = new float3
                            (
                                renderMesh.mesh.bounds.size
                            )
                        });

                        dstManager.SetComponentData(monsters[j + monsterOffset], new PhysicsCollider 
                        { 
                            Value = BoxCollider.Create
                            (
                                CollisionGeomeotrySingleton.Instance.CreateOrGetBoxGeometry(spawnInWorldMonsters[i].ColliderSize), 
                                CollisionFilterSingleton.Instance.BelongsToMonsterFilter
                            )
                        });

                        dstManager.AddSharedComponentData(monsters[j + monsterOffset], renderMesh);
                    }

                    monsterOffset += this.spawnInWorldMonsters[i].SpawnAmount;
                }
            }
        }
    }
}
