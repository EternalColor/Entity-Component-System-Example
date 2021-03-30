using System.Linq;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Deletion;
using FindTheIdol.Components.Inputs;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Players;
using FindTheIdol.Utilities.Constants;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

namespace FindTheIdol.Systems.Inputs
{
    [UpdateAfter(typeof(InputGatherSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(InputClearSystem))]
    public class InputPickupWorldItemSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            NativeArray<Entity> pickupableItems = this.GetEntityQuery(new ComponentType[] { this.EntityManager.GetPickupableItemIdentifierFlat() }).ToEntityArray(Allocator.TempJob);

            this.Entities
                .WithAll<PlayerTag>()
                .ForEach((DynamicBuffer<InputData> inputBuffer, DynamicBuffer<InventoryItem> playerInventoryItems) => 
                {
                    for(int i = 0; i  < inputBuffer.Length; ++i)
                    {
                        InputData input = inputBuffer[i];              

                        if(input.PickupItem)
                        {
                            foreach(Entity itemEntity in pickupableItems)
                            {
                                if(playerInventoryItems.Length < GameRelatedConstants.PlayerTotalItemSlots)
                                {
                                    this.PostUpdateCommands.RemoveComponent<ItemPickupableTag>(itemEntity);

                                    //Make item ready for inventory
                                    InventoryItem inventoryItem = default;

                                    //Copy over the item info
                                    inventoryItem.ItemInfo = this.EntityManager.GetComponentData<ItemInfo>(itemEntity);
                                    inventoryItem.SharedRenderMeshComponentIndex = this.EntityManager.GetSharedComponentDataIndex<RenderMesh>(itemEntity);

                                    //Remove Item Entity from world after its added to player inventory
                                    playerInventoryItems.Add(inventoryItem);
                                    this.PostUpdateCommands.AddComponent<DeletionTag>(itemEntity);
                                }
                            }
                        }            
                    }
                });

            pickupableItems.Dispose();
        }
    }
}