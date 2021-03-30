using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Deletion;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Players;
using FindTheIdol.MonoBehaviours.Items;
using FindTheIdol.Utilities.Constants;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

namespace FindTheIdol.Systems.Items
{

    //Items that are set as starting items or are crafted or are from a savegame, that need to be added directly
    public class ItemDirectlyToInventorySystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            NativeArray<Entity> itemsDirectlyToInventory = this.GetEntityQuery(new ComponentType[] { this.EntityManager.GetItemDirectlyToInventoryIdentifierFlat() }).ToEntityArray(Allocator.TempJob);

            this.Entities.WithAll<PlayerTag>().ForEach((DynamicBuffer<InventoryItem> playerInventoryItems) => 
            {
                foreach(Entity itemEntity in itemsDirectlyToInventory)
                {
                    if(playerInventoryItems.Length < GameRelatedConstants.PlayerTotalItemSlots)
                    {
                        this.PostUpdateCommands.RemoveComponent<ItemDirectlyToInventoryTag>(itemEntity);

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
            });

            itemsDirectlyToInventory.Dispose();
        }
    }
}