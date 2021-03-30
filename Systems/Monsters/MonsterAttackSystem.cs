using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Monsters;
using FindTheIdol.Components.Players;
using FindTheIdol.Systems.Players;
using Unity.Burst;
using Unity.Entities;

namespace FindTheIdol.Systems.Monsters
{
    [BurstCompile]
    [UpdateBefore(typeof(PlayerTakeDamageSystem))]
    public class MonsterAttackSystem : SystemBase
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

            //Assumes there is exactly 1 player 
            Entity playerEntity = this.playerQuery.GetSingletonEntity();

            EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        
            this.Entities
                .WithAll<MonsterAttackTag>()
                .ForEach((Entity monster, int entityInQueryIndex, ref MonsterInfo monsterInfo) =>
                {
                    if(monsterInfo.MonsterAttackIntervalCurrent >= monsterInfo.MonsterAttackIntervalInSeconds)
                    {
                        entityCommandBuffer.AddComponent<PlayerTakeDamageComponent>(entityInQueryIndex, playerEntity, new PlayerTakeDamageComponent { Damage = monsterInfo.Damage });
                        entityCommandBuffer.RemoveComponent<MonsterAttackTag>(entityInQueryIndex, monster);
                        monsterInfo.MonsterAttackIntervalCurrent = 0f;
                    }
                    else
                    {
                        monsterInfo.MonsterAttackIntervalCurrent += deltaTime;
                    }
                }).ScheduleParallel();

            this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

            this.Dependency.Complete();
        }
    }
}
