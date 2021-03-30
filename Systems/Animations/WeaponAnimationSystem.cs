using System;
using System.Collections;
using System.Collections.Generic;
using FindTheIdol.Components.Animations;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Deletion;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.Weapons;
using FindTheIdol.Utilities.Animations;
using FindTheIdol.Utilities.Geometry;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

namespace FindTheIdol.Systems.Animations
{
    [BurstCompile]
    public class WeaponAnimationSystem : ComponentSystem
    {
        private const int MeleeAttackCycles = 2;

        private const float RangedAttackInterval = 1.3f;

        private const int MagicAttackCycles = 2;

        private EntityQuery playerQuery;

        private float3 originPosition = float3.zero;
        
        private float3 destinationPosition = float3.zero;

        private quaternion destinationRotation = quaternion.identity;
        
        private float currentRangedAttackTimeAccumulator = 0f;

        private float currentInterpolator = 0f;
        
        private int attackCycleCounter = 0;

        private bool doneMeleeAttackCycling = false;

        private bool resetMagicAttackIsDestinationRotationSet = false;

        private Entity arrowEntity;

        private Entity magicAnimationEntity;

        protected override void OnCreate()
        {
            this.playerQuery = this.GetEntityQuery(this.EntityManager.GetPlayerIdentifier());
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entity playerEntity = this.playerQuery.GetSingletonEntity();
            Rotation playerRotation = this.EntityManager.GetComponentData<Rotation>(playerEntity);
            PlayerBodyAnchorComponent playerBodyAnchorComponent = this.EntityManager.GetComponentData<PlayerBodyAnchorComponent>(playerEntity);

            this.Entities.WithAll<CurrentlyAnimating>().WithAll<WeaponEquippedTag>().ForEach((Entity weapon, ref Translation position, ref Rotation rotation, ref ItemInfo itemInfo) => 
            {
                switch(itemInfo.PreservedWeaponAttackType)
                {
                    case WeaponAttackType.Melee:
                        //Swap destination and origin with even/odd cyclecounter comparison -> swaps everytime when we increment cycleCounter
                        if(this.attackCycleCounter % 2 == 0)
                        {
                            this.originPosition = playerBodyAnchorComponent.LeftHandAnchorPosition.Value;
                            this.destinationPosition = playerBodyAnchorComponent.WeaponAttackDestinationPosition.Value;
                        }
                        else
                        {
                            this.originPosition = playerBodyAnchorComponent.WeaponAttackDestinationPosition.Value;
                            this.destinationPosition = playerBodyAnchorComponent.LeftHandAnchorPosition.Value;
                        }

                        position.Value = math.lerp(this.originPosition, this.destinationPosition, this.currentInterpolator);
                        this.currentInterpolator += deltaTime * itemInfo.Speed;
                        if(this.currentInterpolator >= 1.0f)
                        {
                            this.currentInterpolator = 0f;
                            ++this.attackCycleCounter;

                            if(this.attackCycleCounter >= WeaponAnimationSystem.MeleeAttackCycles)
                            {
                                this.PostUpdateCommands.RemoveComponent<CurrentlyAnimating>(weapon);
                                this.attackCycleCounter = 0;
                            }
                        }
                        break;

                    case WeaponAttackType.Ranged:
                        if(this.currentRangedAttackTimeAccumulator >= WeaponAnimationSystem.RangedAttackInterval)
                        {
                            //Get arrow mesh and shoot it in parabolic trajectory
                            this.PostUpdateCommands.CreateEntity(this.EntityManager.CreateRangedAttackAnimationArchetype());

                            this.PostUpdateCommands.SetComponent<Translation>(arrowEntity, new Translation { Value = position.Value });
                            this.PostUpdateCommands.SetComponent<Rotation>(arrowEntity, new Rotation { Value = rotation.Value });

                            this.PostUpdateCommands.SetComponent<NonUniformScale>(arrowEntity, new NonUniformScale { Value = new float3(itemInfo.Range.x, itemInfo.Range.y, itemInfo.Range.z) });

                            this.PostUpdateCommands.SetComponent<PhysicsVelocity>(arrowEntity, new PhysicsVelocity
                            { 
                                Linear = new float3(itemInfo.Speed, 0, itemInfo.Speed)
                            });

                            RenderMesh renderMesh = new RenderMesh 
                            { 
                                mesh = MeshFactory.Instance.GetOrCreateArrowMesh(),
                                material = WeaponAnimationMaterialFactory.Instance.GetOrCreateWeaponAnimationMaterial(WeaponAnimationMaterialIndex.RangedAttackMaterial)
                            };

                            this.PostUpdateCommands.SetSharedComponent<RenderMesh>(arrowEntity, renderMesh);

                            this.PostUpdateCommands.SetComponent<PhysicsCollider>(arrowEntity, new PhysicsCollider 
                            {
                                Value = BoxCollider.Create
                                (
                                    CollisionGeomeotrySingleton.Instance.CreateOrGetBoxGeometry(renderMesh.mesh.bounds.size), 
                                    CollisionFilterSingleton.Instance.BelongsToWeaponFilter
                                )
                            });

                            this.PostUpdateCommands.RemoveComponent<CurrentlyAnimating>(weapon);

                            this.currentRangedAttackTimeAccumulator = 0f;
                        }
                        else
                        {
                            this.currentRangedAttackTimeAccumulator += deltaTime;
                        }
                        break;

                    //TODO: Properly implement rotation and attack magic weapon
                    case WeaponAttackType.Magic:
                        //Swap destination and origin with even/odd cyclecounter comparison -> swaps everytime when we increment cycleCounter
                        //Rotate down 90 degrees starting at player rotation (hence multiplying), then rotate back
                        if(this.attackCycleCounter % 2 == 0)
                        {
                            this.destinationRotation = math.mul(quaternion.RotateX(90), playerRotation.Value);
                            this.magicAnimationEntity = Entity.Null;
                        }
                        else
                        {
                            this.destinationRotation = math.mul(quaternion.identity, playerRotation.Value);
                            
                            //Start magic attack animation (mesh "shooting" out of magic weapon), start it once, because magicAnimationEntity can either be null or not (initialize once, later on set to null again)
                            if(this.magicAnimationEntity == Entity.Null)
                            {
                                this.magicAnimationEntity = this.PostUpdateCommands.CreateEntity(this.EntityManager.CreateMagicAttackAnimationArchetype());

                                this.PostUpdateCommands.SetComponent<Translation>(magicAnimationEntity, new Translation { Value = position.Value });
                                this.PostUpdateCommands.SetComponent<Rotation>(magicAnimationEntity, new Rotation { Value = rotation.Value });

                                this.PostUpdateCommands.SetComponent<NonUniformScale>(magicAnimationEntity, new NonUniformScale { Value = new float3(itemInfo.Range.x, itemInfo.Range.y, itemInfo.Range.z) });

                                this.PostUpdateCommands.SetComponent<PhysicsCollider>(magicAnimationEntity, new PhysicsCollider 
                                {
                                    Value = BoxCollider.Create
                                    (
                                        CollisionGeomeotrySingleton.Instance.CreateOrGetBoxGeometry(itemInfo.Range), 
                                        CollisionFilterSingleton.Instance.BelongsToWeaponFilter
                                    )
                                });

                                this.PostUpdateCommands.SetSharedComponent<RenderMesh>(magicAnimationEntity, new RenderMesh 
                                { 
                                    mesh = MeshFactory.Instance.GetOrCreateCube(itemInfo.Range.x, itemInfo.Range.y, itemInfo.Range.z),
                                    material = WeaponAnimationMaterialFactory.Instance.GetOrCreateWeaponAnimationMaterial(WeaponAnimationMaterialIndex.MagicAttackMaterial)
                                });
                            }
                        }

                        rotation.Value = math.slerp(rotation.Value, this.destinationRotation, this.currentInterpolator);
                        
                        this.currentInterpolator += deltaTime * itemInfo.Speed;

                        if(this.currentInterpolator >= 1.0f)
                        {
                            this.currentInterpolator = 0f;
                            ++this.attackCycleCounter;

                            if(this.attackCycleCounter >= WeaponAnimationSystem.MagicAttackCycles)
                            {
                                this.PostUpdateCommands.RemoveComponent<CurrentlyAnimating>(weapon);
                            
                                this.attackCycleCounter = 0;

                                if(this.magicAnimationEntity != Entity.Null)
                                {
                                    this.PostUpdateCommands.AddComponent<DeletionTag>(magicAnimationEntity);
                                    this.magicAnimationEntity = Entity.Null;
                                }
                            }
                        }
                        break;
                }
            }); 
        }
    }
}
