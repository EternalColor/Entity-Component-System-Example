using System.Collections;
using System.Collections.Generic;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Monsters;
using FindTheIdol.Components.Music;
using FindTheIdol.Components.Pathfinding;
using FindTheIdol.Components.Seeds;
using FindTheIdol.Components.World;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FindTheIdol.Systems.Monsters
{
    //This system chooses actions for monsters (e. g next pathfinding position, so the monster move has to run afterwards)
    [UpdateBefore(typeof(MonsterPathfindingSystem))]
    [UpdateBefore(typeof(MonsterMovementSystem))]
    [BurstCompile]
    public class MonsterActionSystem : SystemBase
    {
        //Random chance on a fixed timer to prevent system based randomnes (more chances = more hits).
        private const float UpdateInterval = 0.25f;
        private float currentTime = UpdateInterval;

        private EntityQuery playerQuery;        
        
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            this.playerQuery = this.GetEntityQuery(this.EntityManager.GetPlayerIdentifier());
            
            // Find the ECB system once and store it for later usage
            this.endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            this.currentTime += deltaTime;

            if(this.currentTime >= UpdateInterval)
            {
                Entity seedEntity = this.GetEntityQuery(this.EntityManager.GetSeedIdentifier()).GetSingletonEntity();
                Random random = new Random(this.EntityManager.GetComponentData<Seed>(seedEntity).Value);

                Entity worldBoundsEntity = this.GetEntityQuery(this.EntityManager.GetWorldBoundsIdentifier()).GetSingletonEntity();
                WorldBounds worldBounds = this.EntityManager.GetComponentData<WorldBounds>(worldBoundsEntity);

                EntityArchetype sfxArchetypeForJob = this.EntityManager.CreateSFXArchetype();

                //Assumes there is exactly 1 player 
                Entity playerEntity = this.playerQuery.GetSingletonEntity();
                Translation playerPosition = this.EntityManager.GetComponentData<Translation>(playerEntity);
                ComponentDataFromEntity<MonsterRunTowardsPlayerTag> monsterRunTowardsPlayerComponentDataFromEntity = this.GetComponentDataFromEntity<MonsterRunTowardsPlayerTag>(true);

                EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

                this.Entities
                    .WithAll<MonsterTag>()
                    .ForEach((Entity monster, int entityInQueryIndex, ref PathfindingParameters pathfindingParameters, in Translation position, in MonsterInfo monsterInfo) =>
                    {
                        //No attack tag yet added and player is withing agression radius
                        //Run towards player on grid, afterwards in MovementSystem, we run to the player position -> here we only run to the same grid cell.
                        if(!monsterRunTowardsPlayerComponentDataFromEntity.HasComponent(monster) && math.distance(position.Value, playerPosition.Value) <= monsterInfo.MonsterAgressionRadius)
                        {
                            entityCommandBuffer.AddComponent(entityInQueryIndex, monster, new MonsterRunTowardsPlayerTag());

                            //Choose playerPosition as destination on grid. Pathfinding grid is on x, z components (2d)
                            pathfindingParameters.StartPosition = worldBounds.WorldSpaceToCell(position.Value);
                            pathfindingParameters.EndPosition = worldBounds.WorldSpaceToCell(playerPosition.Value);

                            pathfindingParameters.NeedsPathfindingCalculation = true;

                            //Add Monster Growl SFX
                            Entity sfx = entityCommandBuffer.CreateEntity(entityInQueryIndex, sfxArchetypeForJob);
                            entityCommandBuffer.SetComponent<SFXComponent>(entityInQueryIndex, sfx, new SFXComponent { SFXType = SFXType.MonsterGrowl });
                        }
                        else if(monsterRunTowardsPlayerComponentDataFromEntity.HasComponent(monster) && math.distance(position.Value, playerPosition.Value) > monsterInfo.MonsterAgressionRadius)
                        {
                            entityCommandBuffer.RemoveComponent(entityInQueryIndex, monster, ComponentType.ReadOnly<MonsterRunTowardsPlayerTag>());
                        }
                        
                        //0.01% chance monster chooses new target node to walk to
                        if(!monsterRunTowardsPlayerComponentDataFromEntity.HasComponent(monster) && random.NextFloat() < 0.0001f)
                        {
                            //Choose position on grid. Pathfinding grid is on x, z components (2d)
                            pathfindingParameters.StartPosition = worldBounds.WorldSpaceToCell(position.Value);
                            pathfindingParameters.EndPosition = random.NextInt2(new int2(0,0), worldBounds.XZGridSize);

                            pathfindingParameters.NeedsPathfindingCalculation = true;
                        }
                    })
                    .WithReadOnly(monsterRunTowardsPlayerComponentDataFromEntity)
                    .ScheduleParallel();

                this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

                this.Dependency.Complete();
            }
        }
    }
}
