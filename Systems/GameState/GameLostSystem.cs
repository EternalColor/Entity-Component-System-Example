using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.GameState;
using FindTheIdol.Components.Players;
using Unity.Burst;
using Unity.Entities;

namespace FindTheIdol.Systems.GameState
{
    [BurstCompile]
    public class GameLostSystem : SystemBase
    {
        private EntityQuery gameStateQuery;

        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            this.gameStateQuery = this.GetEntityQuery(this.EntityManager.GetGameStateIdentifier());
            this.endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            Entity gameStateEntity = this.gameStateQuery.GetSingletonEntity();
            GameStateComponent gameStateComponent = this.EntityManager.GetComponentData<GameStateComponent>(gameStateEntity);

            EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            this.Entities.WithAll<PlayerTag>().ForEach((int entityInQueryIndex, in PlayerInfo playerInfo) => 
            {
                if(playerInfo.Health <= 0f)
                {   
                    gameStateComponent.GameStateEnum = GameStateEnum.Lost;

                    entityCommandBuffer.SetComponent(entityInQueryIndex, gameStateEntity, gameStateComponent);
                }   
            }).ScheduleParallel();

            this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

            this.Dependency.Complete();
        }
    }
}
