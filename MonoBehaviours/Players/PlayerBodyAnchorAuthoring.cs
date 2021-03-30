using FindTheIdol.Components.Players;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FindTheIdol.MonoBehaviours.Players
{
    [RequiresEntityConversion]
    public class PlayerBodyAnchorAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {  
        [UnityEngine.SerializeField]
        private UnityEngine.Transform leftHandAnchor;

        [UnityEngine.SerializeField]
        private UnityEngine.Transform rightHandAnchor;

        [UnityEngine.SerializeField]
        private UnityEngine.Transform weaponAttackDestinationAnchor;

        [UnityEngine.SerializeField]
        private UnityEngine.Transform shieldBlockDestinationAnchor;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new PlayerBodyAnchorComponent
            {
                LeftHandAnchorPosition = new Translation { Value = this.leftHandAnchor.position },
                LeftHandAnchorRotation = new Rotation { Value = this.leftHandAnchor.rotation },
                LeftHandAnchorOffset = this.leftHandAnchor.localPosition,

                RightHandAnchorPosition = new Translation { Value = this.rightHandAnchor.position },
                RightHandAnchorRotation = new Rotation { Value = this.rightHandAnchor.rotation },
                RightHandAnchorOffset = this.rightHandAnchor.localPosition,

                WeaponAttackDestinationPosition = new Translation { Value = this.weaponAttackDestinationAnchor.position },
                WeaponAttackDestinationRotation = new Rotation { Value = this.weaponAttackDestinationAnchor.rotation },
                WeaponAttackAnchorOffset = this.weaponAttackDestinationAnchor.localPosition,

                ShieldBlockDestinationAnchorPosition = new Translation { Value = this.shieldBlockDestinationAnchor.position },
                ShieldBlockAttackDestinationRotation = new Rotation { Value = this.shieldBlockDestinationAnchor.rotation },
                ShieldBlockAnchorOffset = this.shieldBlockDestinationAnchor.localPosition
            });
        }
    }
}