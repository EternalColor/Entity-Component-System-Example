using System;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Monsters;
using Unity.Mathematics;

namespace FindTheIdol.MonoBehaviours.Monsters
{
    [Serializable]
    public struct MonsterDTO
    {
        public MonsterInfo MonsterInfo;
        
        public ItemType[] DropItems;

        public UnityEngine.Mesh Mesh;

        public UnityEngine.Material Material;

        //Sadly Mesh.bounds does not work here
        public float3 ColliderSize;
        
        public int SpawnAmount;
    }
}