using System.Collections.Generic;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Monsters;
using FindTheIdol.Components.Weapons;
using FindTheIdol.Systems.Animations;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace FindTheIdol.Systems.Players
{
    [UpdateAfter(typeof(BuildPhysicsWorld))]
    [UpdateAfter(typeof(WeaponAnimationSystem))]
    [BurstCompile]
    public class PlayerDealDamageSystem : SystemBase
    {
        private BuildPhysicsWorld physicsWorld;
        
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        private EntityQuery magicAttackEntityQuery;

        protected override void OnCreate()
        {
            this.physicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            this.endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            this.magicAttackEntityQuery = this.GetEntityQuery(this.EntityManager.GetMagicAttackAnimationIdentifier());
        }
        protected override void OnUpdate()
        {
            Entity magicAttackEntity = Entity.Null;
            if(this.magicAttackEntityQuery.CalculateEntityCount() > 0)
            {
                magicAttackEntity = this.magicAttackEntityQuery.GetSingletonEntity();
            }

            ComponentDataFromEntity<PhysicsCollider> magicAttackPhysicsColliderFromEntity = this.GetComponentDataFromEntity<PhysicsCollider>();

            PhysicsWorld physiscWorldForJob = this.physicsWorld.PhysicsWorld;

            //SystemBase Dependency for the Physics system -> This system must wait for the phyiscs world to complete in order to create a new batch of parallel jobs
            this.Dependency = JobHandle.CombineDependencies(this.Dependency, this.physicsWorld.FinalJobHandle);

            EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            this.Dependency = JobHandle.CombineDependencies(this.Dependency,
            this.Entities.WithAll<WeaponAttackTag>().ForEach((Entity weapon, int entityInQueryIndex, ref Translation position, in Rotation rotation, in PhysicsCollider collider, in ItemInfo itemInfo) => 
            {
                switch(itemInfo.PreservedWeaponAttackType)
                {
                    case WeaponAttackType.Melee:
                        //Melee attack only hits first monster, then it stops (maybe later on there are melee weapons who can punch through enemies)
                        Entity hitEnemy = CollisionHelper.ColliderCast(position.Value, position.Value, rotation.Value, physiscWorldForJob.CollisionWorld, collider, physiscWorldForJob);
                        if(hitEnemy != Entity.Null)
                        {
                            entityCommandBuffer.AddComponent(entityInQueryIndex, hitEnemy, new MonsterTakeDamageComponent { Damage = itemInfo.Damage });
                        }
                        break;
                    case WeaponAttackType.Ranged:
                        break;

                    case WeaponAttackType.Magic:
                        if(magicAttackEntity != Entity.Null)
                        {
                            PhysicsCollider magicAttackCollider = magicAttackPhysicsColliderFromEntity[magicAttackEntity];
                            NativeList<Entity> allHitEntities = new NativeList<Entity>(Allocator.Temp);

                            //WeaponAttackAnimation entity is not rotated -> we assume quaternion identity
                            //Attack goes through enemies (can hit multiple)
                            if(CollisionHelper.ColliderCastAllTargets(position.Value, itemInfo.Range, quaternion.identity, physiscWorldForJob.CollisionWorld, collider, ref allHitEntities))
                            {
                                for(int i = 0; i < allHitEntities.Length; ++i)
                                {
                                    //Add damage to all monsters that got hit
                                    entityCommandBuffer.AddComponent(entityInQueryIndex, allHitEntities[i], new MonsterTakeDamageComponent { Damage = itemInfo.Damage });
                                }
                            }

                            allHitEntities.Dispose();
                        }
                        break;

                    //No default case, because we compare to a enum
                }

                entityCommandBuffer.RemoveComponent<WeaponAttackTag>(entityInQueryIndex, weapon);
            })
            .WithReadOnly(magicAttackPhysicsColliderFromEntity)
            .WithReadOnly(physiscWorldForJob)
            .ScheduleParallel(this.Dependency));

            this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

            this.Dependency.Complete();
        }
    }
}