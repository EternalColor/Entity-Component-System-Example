using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Deletion;
using FindTheIdol.Components.Monsters;
using FindTheIdol.Components.Music;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.Spawning;
using FindTheIdol.Systems.Players;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace FindTheIdol.Systems.Monsters
{
    [BurstCompile]
    [UpdateAfter(typeof(PlayerDealDamageSystem))]
    public class PlayerTakeDamageSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            this.endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            EntityArchetype sfxArchetypeForJob = this.EntityManager.CreateSFXArchetype();
            
            this.Dependency = JobHandle.CombineDependencies(this.Dependency,
            this.Entities
                .WithAll<MonsterTag>()
                .ForEach((Entity monster, DynamicBuffer<DropItem> dropItems, int entityInQueryIndex, ref MonsterInfo monsterInfo, ref MonsterTakeDamageComponent damageComponent) =>
                {   
                    monsterInfo.Health -= damageComponent.Damage;
                    entityCommandBuffer.RemoveComponent<PlayerTakeDamageComponent>(entityInQueryIndex, monster);

                    //Monster with no health will get destroyed (Death)
                    if(monsterInfo.Health <= 0f)
                    {
                        entityCommandBuffer.AddComponent<DeletionTag>(entityInQueryIndex, monster);
                    }

                    //Add Monster Take Damage SFX
                    Entity sfx = entityCommandBuffer.CreateEntity(entityInQueryIndex, sfxArchetypeForJob);
                    entityCommandBuffer.SetComponent<SFXComponent>(entityInQueryIndex, sfx, new SFXComponent { SFXType = SFXType.MonsterTakeDamage });
                }).ScheduleParallel(this.Dependency));
               
            this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

            this.Dependency.Complete();
        }
    }
}