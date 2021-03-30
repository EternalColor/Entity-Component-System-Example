using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Music;
using FindTheIdol.Components.Players;
using FindTheIdol.Systems.Monsters;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace FindTheIdol.Systems.Players
{
    [BurstCompile]
    [UpdateAfter(typeof(MonsterAttackSystem))]
    public class PlayerTakeDamageSystem : SystemBase
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
            //Assumes there is exactly 1 player 
            Entity playerEntity = this.playerQuery.GetSingletonEntity();
            
            EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            EntityArchetype sfxArchetypeForJob = this.EntityManager.CreateSFXArchetype();
            
            this.Dependency = JobHandle.CombineDependencies(this.Dependency,
            this.Entities
                .WithAll<PlayerTag>()
                .ForEach((int entityInQueryIndex, ref PlayerInfo playerInfo, in PlayerTakeDamageComponent playerTakeDamageComponent) =>
                {   
                    playerInfo.Health -= playerTakeDamageComponent.Damage;

                    entityCommandBuffer.RemoveComponent<PlayerTakeDamageComponent>(entityInQueryIndex, playerEntity);

                    //Add Negative SFX
                    Entity sfx = entityCommandBuffer.CreateEntity(entityInQueryIndex, sfxArchetypeForJob);
                    entityCommandBuffer.SetComponent<SFXComponent>(entityInQueryIndex, sfx, new SFXComponent { SFXType = SFXType.Negative });
                }).ScheduleParallel(this.Dependency));

            this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

            this.Dependency.Complete();
        }
    }
}