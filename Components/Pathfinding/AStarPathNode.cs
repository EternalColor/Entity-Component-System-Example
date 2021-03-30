using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FindTheIdol.Components.Pathfinding
{
    public struct PathNode : IComponentData
    {
        public int X;
        public int Y;
        public int Index;

        public int GCost;

        public int HCost; 

        public int FCost => this.GCost + this.HCost;

        public bool IsTraversable;

        public int PreviousNodeIndex;
    }
}