using FindTheIdol.Components.Weapons;
using FindTheIdol.Utilities.Constants;
using Unity.Entities;
using Unity.Rendering;

namespace FindTheIdol.Components.Items
{
    [InternalBufferCapacity(GameRelatedConstants.PlayerInventorySize)]
    public struct InventoryItem : IBufferElementData
    {
        public ItemInfo ItemInfo;

        public int SharedRenderMeshComponentIndex;

        public bool AddedToInventory;
    }
}
