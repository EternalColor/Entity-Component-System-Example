using System.Collections;
using System.Collections.Generic;
using FindTheIdol.Components.Pathfinding;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace FindTheIdol.Pathfinding
{
    [BurstCompile]
    public class AStarUtility
    {
        private const int MOVESTRAIGHTCOST = 10;

        //Approximation to sqrt(2) (diagonal of a square), no rounding errors in square-grid systems 
        private const int MOVEDIAGONALLYCOST = (int)(math.SQRT2 * 10);

        public static int CalculateIndex(int x, int y, int gridWidth)
        {
            return x + y * gridWidth;
        }

        public static int CalculateHCost(int2 startPostiion, int2 endPosition)
        {
            int xDistance = math.abs(startPostiion.x - endPosition.x);
            int yDistance = math.abs(startPostiion.y - endPosition.y);
            int remaining = math.abs(xDistance - yDistance);

            return MOVEDIAGONALLYCOST * math.min(xDistance, yDistance) + MOVESTRAIGHTCOST * remaining;
        }

        public static bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
        {
            return gridPosition.x >= 0 &&
                gridPosition.y >= 0 &&
                gridPosition.x < gridSize.x &&
                gridPosition.y < gridSize.y;
        }

        public static PathNode GetLowestFCostNodeIndex(NativeArray<PathNode> openPathNodes, int2 endPosition)
        {
            PathNode lowestCostPathNode = openPathNodes[0];
            lowestCostPathNode.HCost = AStarUtility.CalculateHCost(new int2(lowestCostPathNode.X, lowestCostPathNode.Y), endPosition);
            //Ignore first node, because we cant walk from a to a (atleast it will be ignored outright)
            for(int i = 1; i < openPathNodes.Length; ++i)
            {
                PathNode testPathNode = openPathNodes[i];
                testPathNode.HCost = AStarUtility.CalculateHCost(new int2(testPathNode.X, testPathNode.Y), endPosition);
                if(testPathNode.FCost < lowestCostPathNode.FCost)
                {
                    lowestCostPathNode = testPathNode;
                }
            }
            
            return lowestCostPathNode;
        }
    }
}
