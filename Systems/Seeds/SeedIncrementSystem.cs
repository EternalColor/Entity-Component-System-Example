using FindTheIdol.Components.Seeds;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace FindTheIdol.Systems.Seeds
{
    [BurstCompile]
    public class SeedIncrementingSystem : ComponentSystem
    {
        //Adding System.Random uint offset to Unity.Mathematic.Random seed
        //If i dont do this, the values will be always the same (i think especially the burst compiler optimizes the calls to random in parallel loops -> you get the same numbers every time)
        // ;) comment out and try yourself - all enemies move to the same location :D
        protected override void OnUpdate()
        {
            this.Entities
            .WithAll<Seed>()
            .ForEach((ref Seed seed) => 
            {
                //Adding random offset
                seed.Value += SystemRandomSingleton.Instance.NextUInt;

                //Seed can not be 0
                if(seed.Value == 0)
                {
                    seed.Value = uint.MinValue + 1;
                }
            });
        }
    }
}
