using System;
using FindTheIdol.Components.World;
using Unity.Mathematics;

public static class WorldBoundsExtension
{
    public static int2 WorldSpaceToCell(this WorldBounds worldBounds, float3 worldSpace)
    {
        return new int2
        (
            WorldSpaceToCell(worldSpace.x, worldBounds.XZGridWorldSpaceMin.x, 0, worldBounds.XZGridSize.x, worldBounds),
            WorldSpaceToCell(worldSpace.z, worldBounds.XZGridWorldSpaceMin.y, 0, worldBounds.XZGridSize.y, worldBounds)
        );
    }

    public static float3 CellToWorldSpace(this WorldBounds worldBounds, int2 cell, float overrideY = 0f)
    {
        //Pathfinding grid is on x, z components (2d) but cell is x and y
        return new float3
        (
            CellToWorldSpace(cell.x, worldBounds.XZGridWorldSpaceMin.x, worldBounds),
            overrideY,
            CellToWorldSpace(cell.y, worldBounds.XZGridWorldSpaceMin.y, worldBounds) 
        );
    }

    private static float CellToWorldSpace(int coordinate, int offset, WorldBounds worldBounds)
    {
        return worldBounds.WorldSpaceCellSize * coordinate + offset;
    }

    private static int WorldSpaceToCell(float worldSpaceCoordinate, int offset, int min, int max, WorldBounds worldBounds)
    {
        //Have to clamp the value, otherwise flooring will make certain values fall below 0 or above grid size because the mesh is too big etc.
        return (int)(math.clamp(math.floor((worldSpaceCoordinate - offset) / worldBounds.WorldSpaceCellSize), min, max));
    }
}
