using FindTheIdol.Utilities.Physics.Collision;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FindTheIdol.MonoBehaviours.Players
{
    [RequiresEntityConversion]
    public class PlayerPhysicsAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public unsafe void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            UnityEngine.MeshFilter meshFilter = this.GetComponent<UnityEngine.MeshFilter>();

            PhysicsCollider physicsCollider = new PhysicsCollider
            {
                Value = BoxCollider.Create
                (
                    CollisionGeomeotrySingleton.Instance.CreateOrGetBoxGeometry(meshFilter.mesh.bounds.size), 
                    CollisionFilterSingleton.Instance.BelongsToPlayerFilter
                )
            };

            dstManager.AddComponentData(entity, physicsCollider);
        }
    }
}
