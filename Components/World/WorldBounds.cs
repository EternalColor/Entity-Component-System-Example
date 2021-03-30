using Unity.Entities;
using Unity.Mathematics;

namespace FindTheIdol.Components.World
{
    [GenerateAuthoringComponent]
    public struct WorldBounds : IComponentData
    {
        public int2 XZGridSize;

        public int2 XZGridWorldSpaceMin;

        public int2 XZGridWorldSpaceMax;

        public float WorldSpaceCellSize;
    }
}