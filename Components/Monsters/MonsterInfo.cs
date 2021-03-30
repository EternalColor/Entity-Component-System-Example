
using System;
using FindTheIdol.Components.Items;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FindTheIdol.Components.Monsters
{
    [Serializable]
    public struct MonsterInfo : IComponentData
    {
        public int Health;
        public int Damage;
        public float Speed;
        public float RotationSpeed;
        
        //This radius defines the detection of the enemy, everything inside this radius becomes a target to run to
        public float MonsterAgressionRadius;

        //This is the visual attack radius of the monster (the space the animation takes to play for attack)
        public float MonsterAttackRadius;

        public float MonsterAttackIntervalInSeconds;

        public float MonsterAttackIntervalCurrent; 
    }
}
