using System;
using FindTheIdol.Components.Classes;
using Unity.Entities;

namespace FindTheIdol.Components.Players
{
    [Serializable]
    public struct PlayerInfo : IComponentData
    {
        public ClassType ClassType;

        public int Health;

        public int Mana;

        public int Level;

        public int Courage;

        public int Agility;

        public int Toughness;

        public int Intelligence;

        public int Willpower;

        public int Crafting;

        public int ArrowCount;

        public float WalkSpeed;

        public float RunSpeed;

        public float JumpForce;

        public float LookSpeed;

        public bool OpenInventoryFromInputData;
    }
}
