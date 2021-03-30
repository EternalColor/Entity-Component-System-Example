using System;

namespace FindTheIdol.Utilities.LayerMask
{
    //Can not be called inside job code it makes calls to UnityEngine.LayerMask which run on the main thread
    public struct LayerMaskWrapper
    {
        public static uint NonTraversableMask => Convert.ToUInt32(UnityEngine.LayerMask.GetMask("NonTraversable"));
        
        public static uint PlayerMask => Convert.ToUInt32(UnityEngine.LayerMask.GetMask("Player"));

        public static uint ItemsMask => Convert.ToUInt32(UnityEngine.LayerMask.GetMask("Items"));

        public static uint AltarMask => Convert.ToUInt32(UnityEngine.LayerMask.GetMask("Altar"));

        public static uint PathfindingGridMask => Convert.ToUInt32(UnityEngine.LayerMask.GetMask("PathfindingGrid"));

        public static uint ItemSlot => Convert.ToUInt32(UnityEngine.LayerMask.GetMask("ItemSlot"));

        public static uint Weapon => Convert.ToUInt32(UnityEngine.LayerMask.GetMask("Weapon"));

        public static uint Monster => Convert.ToUInt32(UnityEngine.LayerMask.GetMask("Monster"));
    }
}