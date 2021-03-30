using FindTheIdol.Components.Items;
using Unity.Mathematics;

namespace FindTheIdol.MonoBehaviours.Items
{
    public struct ItemSlotDTO
    {
        public UnityEngine.UI.RawImage ItemSlotRawImage { get; set; }

        public UnityEngine.UI.Button ItemSlotButton { get; set; }

        //Index to later find the original associated inventory item
        public InventoryItem OriginalInventoryItem { get; set; }

        public int Index { get; set; }

        public bool HasItem { get; set; }

        public bool IsEquippedToPlayer { get; set; }
    }
}
