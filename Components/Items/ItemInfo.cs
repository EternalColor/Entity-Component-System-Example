using System;
using FindTheIdol.Components.Armors;
using FindTheIdol.Components.Shields;
using FindTheIdol.Components.Weapons;
using Unity.Entities;
using Unity.Mathematics;

namespace FindTheIdol.Components.Items
{
    [Serializable]
    public struct ItemInfo : IComponentData 
    {
        public ItemType PreservedItemType;

        public WeaponType PreservedWeaponType;

        public WeaponAttackType PreservedWeaponAttackType;

        public ShieldType PreservedShieldType;

        public ArmorType PreservedArmorType;

        public ArmorName PreservedArmorName;

        public bool IsItem => this.PreservedItemType != ItemType.None;

        public bool IsWeapon => this.PreservedWeaponType != WeaponType.None;

        public bool IsShield => this.PreservedShieldType != ShieldType.None;

        public bool IsArmor => this.PreservedArmorType != ArmorType.None && this.PreservedArmorName != ArmorName.None;

        public float Weight;

        public float Speed;

        //assumed origin is pivot
        public float3 Range;

        public int Damage;

        public int DamageReduction;

        //Arrow Cost is how many arrows are consumed
        public int ArrowCost;

        public int ManaCost;
    }
}
