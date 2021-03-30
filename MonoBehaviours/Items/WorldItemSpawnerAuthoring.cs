using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.World;
using FindTheIdol.MonoBehaviours.World;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using static Unity.Physics.Material;

namespace FindTheIdol.MonoBehaviours.Items
{
    [RequiresEntityConversion]
    [UnityEngine.ExecuteInEditMode]
    public class WorldItemSpawnerAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        [UnityEngine.Header("(red box)")]
        [UnityEngine.SerializeField]
        private float3 scale;

        [UnityEngine.Header("(green box)")]
        [UnityEngine.SerializeField]
        private float3 colliderScale;

        [UnityEngine.SerializeField]
        private ItemSpawnDTO[] spawnInWorldItems;

        [UnityEngine.SerializeField]
        private ItemCatalog ItemCatalog;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            WorldBounds worldBounds = WorldBoundsStaticAccessor.WorldBounds;

            //Make sure every type is only once in the list, if there are multiple of the same type, take the first one (distinct by)
            this.spawnInWorldItems = this.spawnInWorldItems.GroupBy(item => item.ItemType).Select(itm => itm.First()).ToArray();
            int totalSpawnCount = this.spawnInWorldItems.Select(item => item.SpawnAmount).Sum();

            Material triggerMaterial = new Material();
            triggerMaterial.Flags = MaterialFlags.IsTrigger;

            using(NativeArray<Entity> items = dstManager.CreateEntity(dstManager.CreateItemArchetype(), totalSpawnCount, Allocator.Temp))
            {
                //The offset (spawn amount) per ItemType can be different for every entry so we save it to a variable.
                int itemOffset = 0;

                //We previously made sure that the premadeItems list is distincy by ItemType, now we iterate over the whole list
                //and spawn "SpawnAmount" of items
                for(int i = 0; i < this.spawnInWorldItems.Length; ++i)
                {
                    //NullReferenceException on purpose if matching entry not found
                    ItemDTO itemInCatalog = ItemCatalog.PremadeItems.Where(item => item.ItemInfo.PreservedItemType == this.spawnInWorldItems[i].ItemType).First();

                    RenderMesh renderMesh = new RenderMesh 
                    { 
                        mesh = itemInCatalog.Mesh,
                        material = itemInCatalog.Material
                    };

                    for(int j = 0; j < this.spawnInWorldItems[i].SpawnAmount; ++j)
                    {
                        dstManager.SetComponentData(items[j + itemOffset], new ItemInfo 
                        { 
                            Weight = itemInCatalog.ItemInfo.Weight,
                            PreservedItemType = itemInCatalog.ItemInfo.PreservedItemType
                        });

                        dstManager.SetComponentData(items[j + itemOffset], new Translation 
                        { 
                            Value = new float3
                            (
                                UnityEngine.Random.Range(worldBounds.XZGridWorldSpaceMin.x, worldBounds.XZGridWorldSpaceMax.x),
                                0f,
                                UnityEngine.Random.Range(worldBounds.XZGridWorldSpaceMin.y, worldBounds.XZGridWorldSpaceMax.y)
                            )
                        });

                        dstManager.SetComponentData(items[j + itemOffset], new NonUniformScale
                        {
                            Value = this.scale
                        });

                        dstManager.SetComponentData(items[j + itemOffset], new PhysicsCollider
                        {
                            Value = BoxCollider.Create
                            (
                                CollisionGeomeotrySingleton.Instance.CreateOrGetBoxGeometry(this.colliderScale), 
                                CollisionFilterSingleton.Instance.CollidesWithPlayerFilter,
                                triggerMaterial
                            ) 
                        });

                        dstManager.SetSharedComponentData<RenderMesh>(items[j + itemOffset], renderMesh);
                    }

                    itemOffset += this.spawnInWorldItems[i].SpawnAmount;
                }
            }
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
