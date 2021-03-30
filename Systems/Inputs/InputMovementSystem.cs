using FindTheIdol.Components.Inputs;
using FindTheIdol.Components.Players;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace FindTheIdol.Systems.Inputs
{
    [BurstCompile]
    [UpdateAfter(typeof(InputGatherSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(InputClearSystem))]
    public class InputMovementSystem : SystemBase
    {
        private BuildPhysicsWorld physicsWorld;

        protected override void OnCreate()
        {
            this.physicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        }

        protected override void OnUpdate()
        {   
            float deltaTime = Time.DeltaTime;

            CollisionWorld collisionWorldForJob = this.physicsWorld.PhysicsWorld.CollisionWorld;
            //SystemBase Dependency for the Physics system -> This system must wait for the phyiscs world to complete in order to create a new batch of parallel jobs
            this.Dependency = JobHandle.CombineDependencies(this.Dependency, this.physicsWorld.FinalJobHandle);

            this.Dependency = JobHandle.CombineDependencies(this.Dependency,
            this.Entities
            .WithAll<PlayerTag>()
            .ForEach((Entity entity, DynamicBuffer<InputData> inputBuffer, ref Translation position, ref Rotation rotation, ref PlayerInfo playerInfo, ref PhysicsVelocity velocity, ref PhysicsMass physicsMass, in PhysicsCollider collider) => 
            {
                //Fix physics axes xz, only rotate on y
                physicsMass.InverseInertia.xz = 0f;

                for(int i = 0; i  < inputBuffer.Length; ++i)
                {
                    InputData input = inputBuffer[i];
        
                    //If is grounded (ColliderCast) -> jump 
                    if(input.Jump && CollisionHelper.ColliderCast(position.Value, position.Value, rotation.Value, collisionWorldForJob, collider))
                    {
                        velocity.Linear.y = playerInfo.JumpForce;
                    }

                    //If player does not move, immediately stop the character, maybe later on make this slow down smoothly.
                    if(input.Horizontal == 0f && input.Vertical == 0f)
                    {
                        velocity.Linear.xz = 0;
                    }

                    //Push in given direction (multiplying direction and translation)
                    if(input.RunButtonIsPressed)
                    {
                        velocity.Linear.xz = new float3(math.mul(rotation.Value, NormalizeInputDirection(input)) * playerInfo.RunSpeed * deltaTime).xz;
                    }
                    else
                    {
                        velocity.Linear.xz = new float3(math.mul(rotation.Value, NormalizeInputDirection(input)) * playerInfo.WalkSpeed * deltaTime).xz;
                    }
                }
            }).ScheduleParallel(this.Dependency));

            this.Dependency.Complete();
        }

        private static float3 NormalizeInputDirection(in InputData input)
        {
            return math.normalizesafe(new float3(input.Horizontal, 0, input.Vertical));
        }
    }
}
