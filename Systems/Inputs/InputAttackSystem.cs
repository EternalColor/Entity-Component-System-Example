using System.Linq;
using FindTheIdol.Components.Animations;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Inputs;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Music;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.Shields;
using FindTheIdol.Components.Weapons;
using FindTheIdol.Utilities.Attributes;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FindTheIdol.Systems.Inputs
{
    [BurstCompile]
    [UpdateAfter(typeof(InputGatherSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class InputAttackSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            this.Entities
                .WithAll<PlayerTag>()
                .ForEach((Entity player, DynamicBuffer<InputData> inputBuffer, ref Translation position, ref Rotation rotation, ref PlayerInfo playerInfo) => 
                {
                    for(int i = 0; i  < inputBuffer.Length; ++i)
                    {
                        InputData input = inputBuffer[i];

                        if(input.WeaponAttack)
                        {
                            this.Entities.WithAll<WeaponEquippedTag>().ForEach((Entity weapon, ref ItemInfo itemInfo) =>
                            {
                                //Only add once and not over and over again, so we check if component was already added
                                //Also only 1 weapon attack per animation cycle
                                if(!this.EntityManager.HasComponent<CurrentlyAnimating>(weapon))
                                {
                                    //if player has no mana tag no attack will occur
                                    switch(itemInfo.PreservedWeaponAttackType)
                                    {
                                        case WeaponAttackType.Melee:
                                            this.AddAttackToWeaponEntity(ref weapon);
                                            //Add Smash SFX
                                            Entity sfx = this.PostUpdateCommands.CreateEntity(this.EntityManager.CreateSFXArchetype());
                                            this.PostUpdateCommands.SetComponent<SFXComponent>(sfx, new SFXComponent { SFXType = SFXType.Smash });
                                            break;
                                        case WeaponAttackType.Ranged:
                                            // >= 0 because there are bows that take no arrows
                                            if(itemInfo.ArrowCost >= 0 && !this.EntityManager.HasComponent<NoArrowTag>(player))
                                            {
                                                this.AddAttackToWeaponEntity(ref weapon);
                                                this.PostUpdateCommands.AddComponent(player, new ConsumeArrowComponent { Amount = itemInfo.ArrowCost });
                                            }
                                            break;
                                        case WeaponAttackType.Magic:
                                             //Magic attacks MUST consume mana, otherwise we assume its an error in data setup
                                            if(itemInfo.ManaCost > 0 && !this.EntityManager.HasComponent<NoManaTag>(player))
                                            {
                                                this.AddAttackToWeaponEntity(ref weapon);
                                                this.PostUpdateCommands.AddComponent(player, new ConsumeManaComponent { Amount = itemInfo.ManaCost });
                                            }
                                            break;

                                        //No default case because we compare against enum
                                    }
                                }
                            });
                        }

                        //Animate and "hold" aslong as shield block is pressed
                        if(input.ShieldBlock)
                        {
                            this.Entities.WithAll<ShieldEquippedTag>().ForEach((Entity shield) =>
                            {
                                this.AddShieldBlockToShieldEntity(ref shield);
                            });
                        }
                        else
                        {
                            this.Entities.WithAll<ShieldEquippedTag>().ForEach((Entity shield) =>
                            {
                                this.RemoveShieldBlockFromShieldEntity(ref shield);
                            });
                        }
                    }
                });
        }

        private void AddAttackToWeaponEntity(ref Entity weapon)
        {
            this.PostUpdateCommands.AddComponent<CurrentlyAnimating>(weapon);
            this.PostUpdateCommands.AddComponent<WeaponAttackTag>(weapon);
        }

        private void AddShieldBlockToShieldEntity(ref Entity shield)
        {
            if(!this.EntityManager.HasComponent<CurrentlyAnimating>(shield))
            {
                this.PostUpdateCommands.AddComponent<CurrentlyAnimating>(shield);
                this.PostUpdateCommands.AddComponent<ShieldBlockTag>(shield);
            }
        }

        private void RemoveShieldBlockFromShieldEntity(ref Entity shield)
        {
            this.PostUpdateCommands.RemoveComponent<CurrentlyAnimating>(shield);
            this.PostUpdateCommands.RemoveComponent<ShieldBlockTag>(shield);
        }
    }
}