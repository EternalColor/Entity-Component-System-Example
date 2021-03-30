using System.Collections;
using System.Collections.Generic;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Monsters;
using FindTheIdol.Components.Pathfinding;
using FindTheIdol.Components.World;
using FindTheIdol.Pathfinding;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Jobs;
using static Unity.Entities.ComponentType;

namespace FindTheIdol.Systems.Monsters
{
    [BurstCompile]
    [UpdateBefore(typeof(MonsterMovementSystem))]
    public class MonsterPathfindingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entity worldBoundsEntity = this.GetEntityQuery(this.EntityManager.GetWorldBoundsIdentifier()).GetSingletonEntity();
            WorldBounds worldBounds = this.EntityManager.GetComponentData<WorldBounds>(worldBoundsEntity);

            //In order to use burst in AStarUtility.cs, this has to be fed in from a non-burst compiled source (otherwise produces runtime error when creating managed array)
            //Possible movement directions
            NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(new int2[] 
            {
                new int2(-1, 0), // LEFT
                new int2(+1, 0), // RIGHT
                new int2(0, +1), // UP
                new int2(0, -1), // DOWN
                new int2(-1, -1),// LEFT DOWN
                new int2(-1, +1),// LEFT UP
                new int2(+1, -1),// RIGHT DOWN
                new int2(+1, +1) // RIGHT UP

            }, Allocator.TempJob);


            //Get the pathnodes as readonly for performancew
            ComponentType pathNodeIdentifier = this.EntityManager.GetPathnodeIdentifier();
            pathNodeIdentifier.AccessModeType = AccessMode.ReadOnly;

            //Path Nodes that were prepared by the MonsterPathfindingGridSystem 
            NativeArray<PathNode> traversableCheckedPathNodeArray = this.GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { pathNodeIdentifier },
            }).ToComponentDataArray<PathNode>(Allocator.TempJob);
            
            this.Entities
                .WithAll<MonsterTag>()
                .ForEach((DynamicBuffer<PathCell> pathCellBuffer, ref PathfindingParameters pathfindingParameters, ref PathFollow pathFollow) => 
                {
                    //Pathfind towards target
                    if(pathfindingParameters.NeedsPathfindingCalculation)
                    {
                        //Find path from startPosition to endPosition, bound by grid size. Add resulting path into PathPoint buffer
                        AStar.FindPath(pathfindingParameters.StartPosition, pathfindingParameters.EndPosition, worldBounds.XZGridSize, pathCellBuffer, neighbourOffsetArray, ref pathFollow, traversableCheckedPathNodeArray);
                        pathfindingParameters.NeedsPathfindingCalculation = false;
                    }
                })
                //Instead of using Dispose, WithDeallocateOnJobCompletion has to be used for paralllelization
                .WithReadOnly(traversableCheckedPathNodeArray)
                .WithDeallocateOnJobCompletion(neighbourOffsetArray)
                .WithDeallocateOnJobCompletion(traversableCheckedPathNodeArray)
                .ScheduleParallel(); 
        }
    }
}