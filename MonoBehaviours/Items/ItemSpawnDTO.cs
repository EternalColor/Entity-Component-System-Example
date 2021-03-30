using System;
using FindTheIdol.Components.Items;

namespace FindTheIdol.MonoBehaviours.Items
{
    [Serializable]
    public struct ItemSpawnDTO
    {
        public ItemType ItemType;

        public int SpawnAmount;
    }
}