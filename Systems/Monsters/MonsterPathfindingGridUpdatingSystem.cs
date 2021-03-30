using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Obstacles;
using FindTheIdol.Components.Pathfinding;
using FindTheIdol.Components.World;
using FindTheIdol.Pathfinding;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;

namespace FindTheIdol.Systems.Monsters
{
    [BurstCompile]
    //Sets up the Grid used in the MonsterPathfindingSystem so has to run before
    [UpdateBefore(typeof(MonsterPathfindingSystem))]
    [UpdateAfter(typeof(BuildPhysicsWorld))]
    public class MonsterPathfindingGridUpdatingSystem : SystemBase
    {
        //Make it so that static collision is filtered out and set this to a reasonable number
        private const float UpdateInterval = 1f;

        //Update upon first iteration
        private float currentTime = UpdateInterval;

        private BuildPhysicsWorld physicsWorld;

        protected override void OnCreate()
        {
            this.physicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        }

        //Calculate the whole pathfinding grid in a fixed interval to save performance.
        protected override void OnUpdate()
        {
            this.currentTime += Time.DeltaTime;
            
            if(this.currentTime >= UpdateInterval)
            {
                Entity worldBoundsEntity = this.GetEntityQuery(this.EntityManager.GetWorldBoundsIdentifier()).GetSingletonEntity();
                WorldBounds worldBounds = this.EntityManager.GetComponentData<WorldBounds>(worldBoundsEntity);

                CollisionWorld collisionWorldForJob = this.physicsWorld.PhysicsWorld.CollisionWorld;

                //SystemBase Dependency for the Physics system -> This system must wait for the phyiscs world to complete in order to create a new parallel jobs
                this.Dependency = JobHandle.CombineDependencies(Dependency, this.physicsWorld.FinalJobHandle);

                //Can not take the collider, because the raycast does not allow to query a object gridbased -> it detects hit only once, even if multiple boxcasts are present
                //So we take the world render bounds and the AABB of the world render bounds
                NativeArray<WorldRenderBounds> worldRenderBoundsOfObstacles = this.GetEntityQuery(this.EntityManager.GetRenderableTypes()
                                                                                .Concat(new ComponentType[] { this.EntityManager.GetObstacleIdentifier() }).ToArray())
                                                                                .ToComponentDataArray<WorldRenderBounds>(Allocator.TempJob);

                this.Dependency = JobHandle.CombineDependencies(this.Dependency,
                this.Entities
                    .ForEach((Entity entity, ref PathNode pathNode) => 
                {
                    //Get AABB from cubic shape of each path node
                    float3 bottomLeft = worldBounds.CellToWorldSpace(new int2(pathNode.X, pathNode.Y));
                    float3 topRight = worldBounds.CellToWorldSpace(new int2(pathNode.X + 1, pathNode.Y +1));
                    float3 center = (bottomLeft + topRight) / 2;

                    AABB pathNodeAABB = new AABB
                    {
                        Center = center,
                        Extents =  worldBounds.WorldSpaceCellSize 
                    };

                    //Check if there is any worldRenderBoundsObstacle that intersects the AABB of pathNode
                    for(int i = 0; i < worldRenderBoundsOfObstacles.Length; ++i)
                    {
                        //Unfortunately have to cast to bounds to get the intersects method, the ecs struct only has a contains which is not the same
                        if(worldRenderBoundsOfObstacles[i].Value.ToBounds().Intersects(pathNodeAABB.ToBounds()))
                        {
                            pathNode.IsTraversable = false;
                            break;
                        }
                    }
                })
                .WithDeallocateOnJobCompletion(worldRenderBoundsOfObstacles)
                .ScheduleParallel(this.Dependency));

                this.Dependency.Complete();
                this.currentTime = 0f;
            }
        }
    }
}
