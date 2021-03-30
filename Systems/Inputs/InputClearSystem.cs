using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Inputs;
using FindTheIdol.Components.Players;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace FindTheIdol.Systems.Inputs
{
    [BurstCompile]
    [UpdateAfter(typeof(InputGatherSystem))]
    [UpdateAfter(typeof(InputMovementSystem))]
    [UpdateAfter(typeof(InputMouseMovementSystem))]
    [UpdateAfter(typeof(InputPickupWorldItemSystem))]
    [UpdateAfter(typeof(InputAttackSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class InputClearSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            this.Entities
                .WithAll<PlayerTag>()
                .ForEach((DynamicBuffer<InputData> inputBuffer) => 
                {
                    inputBuffer.Clear();
                });
        }
    }
}