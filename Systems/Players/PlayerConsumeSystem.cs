using FindTheIdol.Components.Players;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace FindTheIdol.Systems.Players
{
    [BurstCompile]
    public class PlayerConsumeSystem : SystemBase
    {    
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            this.endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            this.Dependency = JobHandle.CombineDependencies(this.Dependency,
            this.Entities.ForEach((int entityInQueryIndex, Entity player, ref PlayerInfo playerInfo, in ConsumeArrowComponent consumeArrowComponent, in ConsumeManaComponent consumeManaComponent) => 
            {
                if(playerInfo.ArrowCount - consumeArrowComponent.Amount >= 0)
                {
                    playerInfo.ArrowCount -= consumeArrowComponent.Amount;
                }
                else
                {
                    entityCommandBuffer.AddComponent<NoArrowTag>(entityInQueryIndex, player);
                }
                entityCommandBuffer.RemoveComponent<ConsumeManaComponent>(entityInQueryIndex, player);

                if(playerInfo.Mana - consumeManaComponent.Amount >= 0)
                {
                    playerInfo.Mana -= consumeManaComponent.Amount;
                }
                else
                {
                    entityCommandBuffer.AddComponent<NoManaTag>(entityInQueryIndex, player);
                }
                entityCommandBuffer.RemoveComponent<ConsumeManaComponent>(entityInQueryIndex, player);
            }).ScheduleParallel(this.Dependency));

            this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

            this.Dependency.Complete();
        }
    }
}