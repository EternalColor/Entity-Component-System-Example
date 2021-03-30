using FindTheIdol.Components.Animations;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.Shields;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FindTheIdol.Systems.Animations
{
    [BurstCompile]
    public class ShieldAnimationSystem : ComponentSystem
    {
        private EntityQuery playerQuery;

        private float3 origin = float3.zero;
        
        private float3 destination = float3.zero;

        private float currentDistanceInterpolator = 0f;

        protected override void OnCreate()
        {
            this.playerQuery = this.GetEntityQuery(this.EntityManager.GetPlayerIdentifier());
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entity playerEntity = this.playerQuery.GetSingletonEntity();
            PlayerBodyAnchorComponent playerBodyAnchorComponent = this.EntityManager.GetComponentData<PlayerBodyAnchorComponent>(playerEntity);

            this.origin = playerBodyAnchorComponent.RightHandAnchorPosition.Value;
            this.destination = playerBodyAnchorComponent.ShieldBlockDestinationAnchorPosition.Value;

            this.Entities.WithAll<CurrentlyAnimating>().WithAll<ShieldEquippedTag>().ForEach((ref Translation position, ref ItemInfo itemInfo) => 
            {
                if(this.currentDistanceInterpolator <= 1.0f)
                {
                    position.Value = math.lerp(this.origin, this.destination, this.currentDistanceInterpolator);

                    this.currentDistanceInterpolator += deltaTime * itemInfo.Speed;
                }
                else
                {
                    position.Value = this.destination;
                }
            });

            //Clean up when no animation (end animation)
            this.Entities.WithNone<CurrentlyAnimating>().WithAll<ShieldEquippedTag>().ForEach((ref Translation position) =>
            {
                position.Value = this.origin;
                this.currentDistanceInterpolator = 0f;
            });
        }
    }
}