using FindTheIdol.Components.Inputs;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Players;
using Unity.Entities;

namespace FindTheIdol.MonoBehaviours.Players
{
    [RequiresEntityConversion]
    public class PlayerBufferAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {  
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddBuffer<InputData>(entity);
            dstManager.AddBuffer<InventoryItem>(entity);
        }
    }
}
