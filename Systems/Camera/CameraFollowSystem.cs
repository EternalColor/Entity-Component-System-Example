
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Cameras;
using FindTheIdol.Components.Players;
using FindTheIdol.Systems.Inputs;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace FindTheIdol.Systems.Camera
{
    [UpdateAfter(typeof(InputClearSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [BurstCompile]
    public class CameraFollowSystem : ComponentSystem
    {
        private EntityQuery playerQuery;

        protected override void OnCreate()
        {
            this.playerQuery = this.GetEntityQuery(this.EntityManager.GetPlayerIdentifier());
        }
        
        protected override void OnUpdate()
        {
            //Follow Player
            Entity player = this.playerQuery.GetSingletonEntity();
            Translation destinationTranslation = this.EntityManager.GetComponentData<Translation>(player);
            Rotation destinationRotation = this.EntityManager.GetComponentData<Rotation>(player);

            this.Entities
                .WithAll<CameraTag>()
                .ForEach((Entity entity, UnityEngine.Transform gameObjectTransform, ref Translation position, ref Rotation rotation) => 
                {
                    position.Value = destinationTranslation.Value;
                    rotation.Value = destinationRotation.Value;

                    //Hybrid way, can not have the camera component in ECS yet, so i have to update the transform of the gameobject.
                    gameObjectTransform.position = position.Value;
                    gameObjectTransform.rotation = rotation.Value;
                });
        }
    }
}
