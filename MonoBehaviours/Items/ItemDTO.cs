using System;
using FindTheIdol.Components.Items;

namespace FindTheIdol.MonoBehaviours.Items
{
    [Serializable]
    public struct ItemDTO
    {
        public ItemInfo ItemInfo;

        public UnityEngine.Mesh Mesh;
        
        public UnityEngine.Material Material;
    }
}