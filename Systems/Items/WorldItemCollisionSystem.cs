using FindTheIdol.Components.Items;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace FindTheIdol.Systems.Items
{
    [UpdateAfter(typeof(BuildPhysicsWorld))]
    [BurstCompile]
    public class WorldItemCollisionSystem : SystemBase
    {
        private BuildPhysicsWorld physicsWorld;
        
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            this.physicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            this.endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            CollisionWorld collisionWorldForJob = this.physicsWorld.PhysicsWorld.CollisionWorld;
            //SystemBase Dependency for the Physics system -> This system must wait for the phyiscs world to complete in order to create a new batch of parallel jobs
            this.Dependency = JobHandle.CombineDependencies(this.Dependency, this.physicsWorld.FinalJobHandle);

            EntityCommandBuffer.Concurrent entityCommandBuffer = this.endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            this.Dependency = JobHandle.CombineDependencies(this.Dependency,
            this.Entities
                .WithAll<ItemTag>()
                .ForEach((Entity entity, int entityInQueryIndex, in Translation position, in Rotation rotation, in PhysicsCollider collider) =>
                {
                    if(CollisionHelper.ColliderCast(position.Value, position.Value, rotation.Value, collisionWorldForJob, collider))
                    {
                        entityCommandBuffer.AddComponent<ItemPickupableTag>(entityInQueryIndex, entity);
                    }
                    else
                    {
                        entityCommandBuffer.RemoveComponent<ItemPickupableTag>(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel(this.Dependency));

            this.endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

            this.Dependency.Complete();
        }
    }
}
