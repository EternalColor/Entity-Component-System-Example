using FindTheIdol.Components.Items;
using FindTheIdol.Utilities.Constants;
using Unity.Entities;

namespace FindTheIdol.Components.Monsters
{
    [InternalBufferCapacity(GameRelatedConstants.MaxDropItems)]
    public struct DropItem : IBufferElementData
    {
        public ItemType ItemType;
    }
}