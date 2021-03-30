using Unity.Entities;

namespace FindTheIdol.MonoBehaviours.Items
{
    [RequiresEntityConversion]
    public class ItemInfoWorldSpaceLabelUIInjectorAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        [UnityEngine.SerializeField]
        private UnityEngine.GameObject pickupItemLabelGameObject;

        private UnityEngine.UI.Text pickupItemText;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        { 
            //Get the UI components and "inject" them into the ecs (UISystem) so we can control them there
            this.pickupItemText = pickupItemLabelGameObject.GetComponent<UnityEngine.UI.Text>();

            dstManager.AddComponentObject(entity, this.pickupItemText);
        }
    }
}
