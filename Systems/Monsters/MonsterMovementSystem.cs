using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Monsters;
using FindTheIdol.Components.Pathfinding;
using FindTheIdol.Components.World;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FindTheIdol.Systems.Monsters
{
    [BurstCompile]
    public class MonsterMovementSystem : SystemBase
    {
        private EntityQuery playerQuery;

        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            this.playerQuery = this.GetEntityQuery(this.EntityManager.GetPlayerIdentifier());
            this.endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entity worldBoundsEntity = this.GetEntityQuery(this.EntityManager.GetWorldBoundsIdentifier()).GetSingletonEntity();
            WorldBounds worldBounds = this.EntityManager.GetComponentData<WorldBounds>(worldBoundsEntity);

            ComponentDataFromEntity<MonsterRunTowardsPlayerTag> monsterRunTowardsEnemyTagComponentDataFromEntity = this.GetComponentDataFromEntity<MonsterRunTowardsPlayerTag>();

            //Assumes there is exactly 1 player 
            Entity playerEntity = this.playerQuery.GetSingletonEntity();
            float3 playerPosition = this.GetComponentDataFromEntity<Translation>()[playerEntity].Value;

            EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            this.Entities
                .WithAll<MonsterTag>()
                .ForEach((Entity monster, DynamicBuffer<PathCell> pathCellBuffer, int entityInQueryIndex, ref PathFollow pathFollow, ref Translation position, ref Rotation rotation, in MonsterInfo monsterInfo) =>
                {
                    if(pathFollow.PathIndex >= 0)
                    {
                        int2 cell = pathCellBuffer[pathFollow.PathIndex].Cell;

                        //Keep y height of monster (position.Value.y)
                        //Keep in mind XZGridWorldSpaceMin is a negative value!
                        float3 targetPosition =  worldBounds.CellToWorldSpace(cell, position.Value.y);
                        float3 targetDirection = math.normalizesafe(targetPosition - position.Value);
                        WalkTowards(ref position, ref rotation, in monsterInfo, in targetPosition, in targetDirection, in deltaTime);

                        if(math.distance(position.Value, targetPosition) < worldBounds.WorldSpaceCellSize / 2)
                        {
                            //Choose next waypoint
                            pathFollow.PathIndex--;
                        }
                    }
                    else
                    {
                        //After following the whole path on the pathfinding grind and the enemy is attacking, walk towards the enemy (player)
                        if(monsterRunTowardsEnemyTagComponentDataFromEntity.HasComponent(monster))
                        {
                            //Monster is within attack range
                            if(math.distance(position.Value, playerPosition) > monsterInfo.MonsterAttackRadius)
                            {
                                float3 targetDirection = math.normalizesafe(playerPosition - position.Value);
                                WalkTowards(ref position, ref rotation, in monsterInfo, in playerPosition, in targetDirection, in deltaTime);
                            }
                            else
                            {
                                entityCommandBuffer.AddComponent<MonsterAttackTag>(entityInQueryIndex, monster);
                            }
                        }
                        else
                        {
                            //Reached the end immediately or invalid movement, so there is no need for the Buffer
                            pathCellBuffer.Clear();
                        }
                    }
                })
                .WithReadOnly(monsterRunTowardsEnemyTagComponentDataFromEntity)
                .ScheduleParallel();

            this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

            this.Dependency.Complete();
        }

        private static void WalkTowards(ref Translation position, ref Rotation rotation, in MonsterInfo monsterInfo, in float3 targetPosition, in float3 targetDirection, in float deltaTime)
        {
            position.Value.xz += new float3(targetDirection * monsterInfo.Speed * deltaTime).xz;

            //Rotate monster towards destination
            //Ignore Y for following rotation (height of the node (y-component))
            rotation.Value = math.slerp(rotation.Value, quaternion.LookRotationSafe
            (
                new float3(targetDirection.x, 0, targetDirection.z),
                UnityEngine.Vector3.up), 
                monsterInfo.RotationSpeed * deltaTime
            );
        }
    }
}
