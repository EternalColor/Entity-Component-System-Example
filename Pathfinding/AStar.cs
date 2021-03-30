using System.Collections;
using System.Collections.Generic;
using FindTheIdol.Components.Pathfinding;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

namespace FindTheIdol.Pathfinding
{
    [BurstCompile]
    public struct AStar
    {
        //Find path from startPosition to endPosition, bound by grid size. Add resulting path into PathCell buffer
        public static void FindPath(int2 startPosition, int2 endPosition, int2 gridSize, DynamicBuffer<PathCell> pathCellBuffer, NativeArray<int2> neighbourOffsetArray, ref PathFollow pathFollow, NativeArray<PathNode> pathNodesTraversableLookupReadOnly)
        {
            //Keeping internal copy of calculated path nodes for thread safety, otherwise multiple threads (jobs) use the pathNodesReadOnly Array directly
            //Using NativeHashMap because it has faster lookup speed (O(1) best) (0(n) worst) than (list and array 0(n) best)
            NativeHashMap<int, PathNode> calculatedPathNodes = new NativeHashMap<int, PathNode>(pathNodesTraversableLookupReadOnly.Length, Allocator.Temp);

            //Add Start Node
            PathNode startNode = pathNodesTraversableLookupReadOnly[AStarUtility.CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
            startNode.GCost = 0;
            startNode.HCost = AStarUtility.CalculateHCost(new int2(startNode.X, startNode.Y), endPosition);
            startNode.PreviousNodeIndex = -1;
            calculatedPathNodes.Add(startNode.Index, startNode);      

            int endNodeIndex = AStarUtility.CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

            if(!calculatedPathNodes.ContainsKey(endNodeIndex))
            {
                calculatedPathNodes.Add(endNodeIndex, pathNodesTraversableLookupReadOnly[endNodeIndex]);
            }

            //Using NativeHashMap because it has faster lookup speed (O(1) best) (0(n) worst) than (list and array 0(n) best)
            NativeHashMap<int, PathNode> openHashMap = new NativeHashMap<int, PathNode>(pathNodesTraversableLookupReadOnly.Length, Allocator.Temp);
            NativeHashMap<int, int> closedHashMap = new NativeHashMap<int, int>(pathNodesTraversableLookupReadOnly.Length, Allocator.Temp);

            openHashMap.Add(startNode.Index, startNode);

            while(openHashMap.Count() > 0)
            {
                NativeArray<PathNode> openPathNodesValueArray = openHashMap.GetValueArray(Allocator.Temp);
                PathNode currentNode = AStarUtility.GetLowestFCostNodeIndex(openPathNodesValueArray, endPosition);
                
                //Overwrite entry
                calculatedPathNodes[currentNode.Index] = currentNode;
            
                //? Reached EndNode
                if(currentNode.Index == endNodeIndex)
                {
                    break;
                }

                //Remove current node from Open Lists
                for(int i = 0; i < openPathNodesValueArray.Length; ++i)
                {
                    if(openPathNodesValueArray[i].Index == currentNode.Index)
                    {
                        openHashMap.Remove(openPathNodesValueArray[i].Index);
                        break;
                    }
                }

                closedHashMap.Add(currentNode.Index, currentNode.Index);

                // Testing all directions
                for(int i = 0; i < neighbourOffsetArray.Length; ++i)
                {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPosition = new int2(currentNode.X + neighbourOffset.x, currentNode.Y + neighbourOffset.y);

                    if(!AStarUtility.IsPositionInsideGrid(neighbourPosition, gridSize))
                    {
                        //Not valid position
                        continue;
                    }

                    int neighbourNodeIndex = AStarUtility.CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

                    if(closedHashMap.ContainsKey(neighbourNodeIndex))
                    {
                        //Already searched this node
                        continue;
                    }

                    PathNode neighbourNode = pathNodesTraversableLookupReadOnly[neighbourNodeIndex];

                    if(!neighbourNode.IsTraversable)
                    {
                        //Can not walk on this node
                        continue;
                    }

                    //Calculate GCost for Neighbour
                    int gCostNext = currentNode.GCost + AStarUtility.CalculateHCost(new int2(currentNode.X, currentNode.Y), neighbourPosition);
                    if(gCostNext < neighbourNode.GCost)
                    {
                        neighbourNode.PreviousNodeIndex = currentNode.Index;
                        neighbourNode.GCost = gCostNext;

                        if(!calculatedPathNodes.ContainsKey(neighbourNode.Index))
                        {
                            calculatedPathNodes.Add(neighbourNode.Index, neighbourNode);
                        }

                        if(!openHashMap.ContainsKey(neighbourNode.Index) )
                        {
                            openHashMap.Add(neighbourNode.Index, neighbourNode);
                        }
                    }
                }
            }

            closedHashMap.Dispose();
            openHashMap.Dispose();

            pathCellBuffer.Clear();
            CalculatePath(calculatedPathNodes, calculatedPathNodes[endNodeIndex], pathCellBuffer, ref pathFollow);

            calculatedPathNodes.Dispose();
        }

        private static void CalculatePath(NativeHashMap<int, PathNode> pathNodeMap, PathNode endNode, DynamicBuffer<PathCell> pathPointBuffer, ref PathFollow pathFollow)
        {
            //Didnt find a path!
            if(endNode.PreviousNodeIndex == -1)
            {
                pathFollow = new PathFollow { PathIndex = -1 };
                return;
            }
            else
            {
                pathPointBuffer.Add(new PathCell{ Cell = new int2(endNode.X, endNode.Y) });

                PathNode currentNode = endNode;

                while(currentNode.PreviousNodeIndex != -1)
                {
                    PathNode previousNode = pathNodeMap[currentNode.PreviousNodeIndex];
                    pathPointBuffer.Add(new PathCell { Cell = new int2(previousNode.X, previousNode.Y)} );
                    currentNode = previousNode;
                }

                pathFollow = new PathFollow { PathIndex = pathPointBuffer.Length - 1 };
            }
        }
    }
}