using FindTheIdol.Components.Items;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.Shields;
using FindTheIdol.Components.Weapons;

namespace FindTheIdol.Utilities.Constants
{
    public sealed class GameRelatedConstants 
    {
        public const int PlayerItemSlotSize = 5;
        
        public const int PlayerInventorySize = 20;

        public const int PlayerTotalItemSlots = PlayerItemSlotSize + PlayerInventorySize;

        public const int InputBufferSize = 64;

        public const int AnimationBufferSize = 20;
        
        public const float WorldSpaceLabelRotationSpeed = 2f;

        public const int ItemSlotSize = 128;

        public const int ItemSlotDepth = 0;

        public const int MaxDropItems = 100;

        public const int MinStatValue = 0;

        public const int MaxStatValue = 20;

        public const int WarriorPointsLeft = 2;

        public const int WarriorMaxHPRaiseOnLevelUp = 20;

        public const int WarriorMaxManaRaiseOnLevelUp = 0;

        public static PlayerInfo PresetWarriorStatsInfo = new PlayerInfo
        {
            //On level 1 we start with the MaxHPRaise amount
            Health = WarriorMaxHPRaiseOnLevelUp,
            Mana = WarriorMaxManaRaiseOnLevelUp,
            Level = 1,
            Courage = 12,
            Agility = 11,
            Toughness = 10,
            Intelligence = 10,
            Willpower = 8,
            Crafting = 10
        };

        public static ItemType[] PresetWarriorItemTypes = new ItemType[]
        {
            ItemType.SmallBrockium
        };

        public static WeaponType[] PresetWarriorWeaponTypes = new WeaponType[]
        {
            WeaponType.Broadsword
        };

        public static ShieldType[] PresetWarriorShieldTypes = new ShieldType[]
        {
            ShieldType.Wood
        };

        public const int RangerPointsLeft = 3;

        public const int RangerMaxHPRaiseOnLevelUp = 14;

        public const int RangerMaxManaRaiseOnLevelUp = 0;

        public static PlayerInfo PresetRangerStatsInfo = new PlayerInfo
        {
            Health = RangerMaxHPRaiseOnLevelUp,
            Mana = RangerMaxManaRaiseOnLevelUp,
            Level = 1,
            Courage = 11,
            Agility = 13,
            Toughness = 11,
            Intelligence = 9,
            Willpower = 8,
            Crafting = 10
        };

        public static ItemType[] PresetRangerItemTypes = new ItemType[]
        {
            ItemType.SmallBrockium
        };

        public static WeaponType[] PresetRangerWeaponTypes = new WeaponType[]
        {
            WeaponType.Shortbow
        };

        //No starting shield
        public static ShieldType[] PresetRangerShieldTypes = new ShieldType[]{};

        public const int MagePointsLeft = 4;

        public const int MageMaxHPRaiseOnLevelUp = 10;

        public const int MageMaxManaRaiseOnLevelUp = 20;        

        public static PlayerInfo PresetMageStatsInfo = new PlayerInfo
        {
            Health = MageMaxHPRaiseOnLevelUp,
            Mana = MageMaxManaRaiseOnLevelUp,
            Level = 1,
            Courage = 9,
            Agility = 11,
            Toughness = 10,
            Intelligence = 13,
            Willpower = 12,
            Crafting = 10
        };

        public static ItemType[] PresetMageItemTypes = new ItemType[]
        {
            ItemType.WoodStick
        };

        public static WeaponType[] PresetMageWeaponTypes = new WeaponType[]
        {
            WeaponType.SparklingStick
        };

        public static ShieldType[] PresetMageShieldTypes = new ShieldType[]
        {
            ShieldType.Wood
        };

        public const int ForsakenPointsLeft = 5;

        public const int ForsakenMaxHPRaiseOnLevelUp = 6;

        public const int ForsakenMaxManaRaiseOnLevelUp = 20;   

        public static PlayerInfo PresetForsakenStatsInfo = new PlayerInfo
        {
            Health = ForsakenMaxHPRaiseOnLevelUp,
            Mana = ForsakenMaxManaRaiseOnLevelUp,
            Level = 1,
            Courage = 7,
            Agility = 8,
            Toughness = 8,
            Intelligence = 14,
            Willpower = 14,
            Crafting = 13
        };

        public static ItemType[] PresetForsakenItemTypes = new ItemType[]
        {
            ItemType.WoodStick
        };

        public static WeaponType[] PresetForsakenWeaponTypes = new WeaponType[]
        {
            WeaponType.Pitchfork
        };

        public static ShieldType[] PresetForsakenShieldTypes = new ShieldType[]
        {
            ShieldType.Wood
        };
    }
}
