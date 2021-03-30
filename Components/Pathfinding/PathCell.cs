using Unity.Entities;
using Unity.Mathematics;

namespace FindTheIdol.Components.Pathfinding
{
    [InternalBufferCapacity(20)]
    public struct PathCell : IBufferElementData
    {
        public int2 Cell;
    }
}
