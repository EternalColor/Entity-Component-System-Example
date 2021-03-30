using Unity.Entities;
using Unity.Mathematics;

namespace FindTheIdol.Components.Pathfinding
{
    //Holds Index of current Node. Used to find Node in BufferArray<Node>
    public struct PathFollow : IComponentData
    {
        public int PathIndex;
    }
}
