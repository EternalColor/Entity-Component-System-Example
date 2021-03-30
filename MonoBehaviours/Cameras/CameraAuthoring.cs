using System.Collections;
using System.Collections.Generic;
using FindTheIdol.Components.Cameras;
using Unity.Entities;

namespace FindTheIdol.MonoBehaviours.Cameras
{
    [RequiresEntityConversion]
    public class CameraAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //Track Camera movement via Object Field (Camera is still GameObject not ECS)
            dstManager.AddComponentObject(entity, transform);
        }
    }
}
