using System.Collections;
using System.Collections.Generic;
using FindTheIdol.Components.Altar;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.GameState;
using FindTheIdol.Components.Music;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace FindTheIdol.Systems.Altar
{
    [UpdateAfter(typeof(BuildPhysicsWorld))]
    [BurstCompile]
    public class PlaceIdolSystem : SystemBase
    {
        private BuildPhysicsWorld physicsWorld;

        private EntityQuery gameStateQuery;

        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            this.physicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            this.endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            this.gameStateQuery = this.GetEntityQuery(this.EntityManager.GetGameStateIdentifier());
        }

        protected override void OnUpdate()
        {
            CollisionWorld collisionWorldForJob = this.physicsWorld.PhysicsWorld.CollisionWorld;
            //SystemBase Dependency for the Physics system -> This system must wait for the phyiscs world to complete in order to create a new batch of parallel jobs
            this.Dependency = JobHandle.CombineDependencies(this.Dependency, this.physicsWorld.FinalJobHandle);

            Entity gameStateEntity = this.gameStateQuery.GetSingletonEntity();
            GameStateComponent gameStateComponent = this.EntityManager.GetComponentData<GameStateComponent>(gameStateEntity);

            EntityArchetype sfxArchetypeForJob = this.EntityManager.CreateSFXArchetype();

            EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            this.Dependency = JobHandle.CombineDependencies(this.Dependency,
            this.Entities.WithAll<AltarTag>().ForEach((int entityInQueryIndex, in Translation position, in Rotation rotation, in PhysicsCollider collider) => 
            {
                if(CollisionHelper.ColliderCast(position.Value, position.Value, rotation.Value, collisionWorldForJob, collider))
                {
                    gameStateComponent.GameStateEnum = GameStateEnum.Win;

                    entityCommandBuffer.SetComponent<GameStateComponent>(entityInQueryIndex, gameStateEntity, gameStateComponent);

                    //Add Fanfare SFX
                    Entity sfx = entityCommandBuffer.CreateEntity(entityInQueryIndex, sfxArchetypeForJob);
                    entityCommandBuffer.SetComponent<SFXComponent>(entityInQueryIndex, sfx, new SFXComponent { SFXType = SFXType.Fanfare });
                }
            }).ScheduleParallel(this.Dependency));

            this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

            this.Dependency.Complete();
        }
    }
}
