using Unity.Entities;
using Unity.Mathematics;

namespace FindTheIdol.Components.Pathfinding
{
    public struct PathfindingParameters : IComponentData
    {
        public int2 StartPosition;
        public int2 EndPosition;

        public bool NeedsPathfindingCalculation;

        public int PathIndex;
    }
}
