using System;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Seeds;
using Unity.Entities;

namespace FindTheIdol.MonoBehaviours.Seeds
{
    [RequiresEntityConversion]
    public class SeedCreatorAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Entity seedEntity = dstManager.CreateEntity(dstManager.CreateSeedArchetype());

            dstManager.SetComponentData(seedEntity, new Seed { Value = SystemRandomSingleton.Instance.NextUInt });
        }

    }
}
