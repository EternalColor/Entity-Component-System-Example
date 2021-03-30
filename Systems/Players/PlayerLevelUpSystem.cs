using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Classes;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.Seeds;
using FindTheIdol.Systems.GameState;
using FindTheIdol.Utilities.Constants;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace FindTheIdol.Systems.Players
{
    [BurstCompile]
    //Needs a chance to level up the player before the game lost condition is evaluated
    [UpdateBefore(typeof(GameLostSystem))]
    public class PlayerLevelUpSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            this.endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            Entity seedEntity = this.GetEntityQuery(this.EntityManager.GetSeedIdentifier()).GetSingletonEntity();
            Random random = new Random(this.EntityManager.GetComponentData<Seed>(seedEntity).Value);

            EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            this.Entities.WithAll<PlayerLevelUpTag>().ForEach((Entity entity, int entityInQueryIndex, ref PlayerInfo playerInfo) => 
            {
                ++playerInfo.Level;       

                switch(playerInfo.ClassType)
                {
                    case ClassType.Warrior:
                        playerInfo.Health += random.NextInt(1, GameRelatedConstants.WarriorMaxHPRaiseOnLevelUp);
                        //Non-magic class
                        playerInfo.Mana += GameRelatedConstants.WarriorMaxManaRaiseOnLevelUp;
                        break;
                    case ClassType.Ranger:
                        playerInfo.Health += random.NextInt(1, GameRelatedConstants.RangerMaxHPRaiseOnLevelUp);
                        //Non-magic class
                        playerInfo.Mana += GameRelatedConstants.RangerMaxManaRaiseOnLevelUp;
                        break;
                    case ClassType.Mage:
                        playerInfo.Health += random.NextInt(1, GameRelatedConstants.MageMaxHPRaiseOnLevelUp);
                        playerInfo.Mana += random.NextInt(1, GameRelatedConstants.MageMaxManaRaiseOnLevelUp);
                        break;
                    case ClassType.Forsaken:
                        playerInfo.Health += random.NextInt(1, GameRelatedConstants.ForsakenMaxHPRaiseOnLevelUp);
                        playerInfo.Mana += random.NextInt(1, GameRelatedConstants.ForsakenMaxManaRaiseOnLevelUp);
                        break;
                }

                entityCommandBuffer.RemoveComponent<PlayerLevelUpTag>(entityInQueryIndex, entity);
            }).ScheduleParallel();

            this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

            this.Dependency.Complete();
        }
    }
}