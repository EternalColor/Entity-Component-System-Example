using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FindTheIdol.Components.Players
{
    public struct PlayerBodyAnchorComponent : IComponentData
    {
        public Translation LeftHandAnchorPosition;

        public Rotation LeftHandAnchorRotation;

        public float3 LeftHandAnchorOffset;

        public Translation RightHandAnchorPosition;

        public Rotation RightHandAnchorRotation;

        public float3 RightHandAnchorOffset;

        public Translation WeaponAttackDestinationPosition;

        public Rotation WeaponAttackDestinationRotation;

        public float3 WeaponAttackAnchorOffset;

        public Translation ShieldBlockDestinationAnchorPosition;

        public Rotation ShieldBlockAttackDestinationRotation;

        public float3 ShieldBlockAnchorOffset;
    }
}