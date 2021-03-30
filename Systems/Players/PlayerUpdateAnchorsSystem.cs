using FindTheIdol.Components.Players;
using FindTheIdol.Systems.Inputs;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FindTheIdol.Systems.Players
{
    [BurstCompile]
    [UpdateAfter(typeof(InputMovementSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    //In ECS there is no parenting, so you have to move the anchors with the player manually.
    public class PlayerUpdateAnchorsSystem : SystemBase
    {
        private Rotation lastRotation;

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            this.Entities.WithAll<PlayerTag>().ForEach((ref PlayerBodyAnchorComponent playerBodyAnchorComponent, ref Translation position, ref Rotation rotation) =>
            {
                //Rotate around player, around y axis,
                //Unfortunately there is no euler angles in the ecs libraryrotate
                playerBodyAnchorComponent.LeftHandAnchorPosition.Value = position.Value + math.mul(rotation.Value, playerBodyAnchorComponent.LeftHandAnchorOffset);
                playerBodyAnchorComponent.LeftHandAnchorRotation.Value = rotation.Value;

                playerBodyAnchorComponent.RightHandAnchorPosition.Value = position.Value + math.mul(rotation.Value, playerBodyAnchorComponent.RightHandAnchorOffset);
                playerBodyAnchorComponent.RightHandAnchorRotation.Value = rotation.Value;   

                playerBodyAnchorComponent.WeaponAttackDestinationPosition.Value = position.Value + math.mul(rotation.Value, playerBodyAnchorComponent.WeaponAttackAnchorOffset);
                playerBodyAnchorComponent.WeaponAttackDestinationRotation.Value = rotation.Value; 

                playerBodyAnchorComponent.ShieldBlockDestinationAnchorPosition.Value = position.Value + math.mul(rotation.Value, playerBodyAnchorComponent.ShieldBlockAnchorOffset);
                playerBodyAnchorComponent.ShieldBlockAttackDestinationRotation.Value = rotation.Value; 
            }).ScheduleParallel();
        }
    }
}