
using System.Linq;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Inputs;
using FindTheIdol.Components.Players;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FindTheIdol.Systems.Inputs
{
    [BurstCompile]
    [UpdateAfter(typeof(InputGatherSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class InputMouseMovementSystem : ComponentSystem
    {
        private float MouseYaw = 0f;

        private float MousePitch = 0f;

        private const float MinimumX = -360F;

        private const float MaximumX = 360F;
        
        private const float MinimumY = -60F;
        
        private const float MaximumY = 60F;

        private quaternion originalRotation;

        private bool copyOriginalRotationOfPlayer = true;

        protected override void OnUpdate()
        {
            this.CopyOriginalRotationOfPlayer();

            this.Entities
                .WithAll<PlayerTag>()
                .ForEach((Entity entity, DynamicBuffer<InputData> inputBuffer, ref Translation position, ref Rotation rotation, ref PlayerInfo playerInfo) => 
                {
                    for(int i = 0; i  < inputBuffer.Length; ++i)
                    {
                        InputData input = inputBuffer[i];

                        ProcessMouseInput(in input, ref rotation, in playerInfo);
                    }
                });
        }

        private void ProcessMouseInput(in InputData input, ref Rotation rotation, in PlayerInfo playerInfo)
        {
            // Read the mouse input axis
            this.MouseYaw += input.MouseX * playerInfo.LookSpeed * Time.DeltaTime;
            this.MousePitch += input.MouseY * playerInfo.LookSpeed * Time.DeltaTime;
            
            this.MouseYaw = ClampAngle(this.MouseYaw, MinimumX, MaximumX);
            this.MousePitch = ClampAngle(this.MousePitch, MinimumY, MaximumY);
            
            UnityEngine.Quaternion xQuaternion = UnityEngine.Quaternion.AngleAxis(this.MouseYaw, UnityEngine.Vector3.up);
            UnityEngine.Quaternion yQuaternion = UnityEngine.Quaternion.AngleAxis(this.MousePitch, -UnityEngine.Vector3.right);
            
            rotation.Value = this.originalRotation * xQuaternion * yQuaternion;
        }

        private void CopyOriginalRotationOfPlayer()
        {
            if(copyOriginalRotationOfPlayer)
            {
                //Combine Player + Spacial Components to get player rotation
                using(NativeArray<Rotation> playerRotations = this.GetEntityQuery(this.EntityManager.GetSpacialTypes()
                .Concat(new ComponentType[] { this.EntityManager.GetPlayerIdentifier() }).ToArray())
                .ToComponentDataArray<Rotation>(Allocator.Temp))
                {
                    //There is only 1 player, wait until player was created
                    if(playerRotations.Length > 0)
                    {
                        this.originalRotation = playerRotations[0].Value;
                        this.copyOriginalRotationOfPlayer  = false;
                    }
                }
            }
        }
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f)
                angle += 360f;
            if (angle > 360f)
                angle -= 360f;
            return math.clamp(angle, min, max);
        }
    }
}
