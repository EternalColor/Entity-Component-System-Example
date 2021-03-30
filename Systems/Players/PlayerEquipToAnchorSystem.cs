using FindTheIdol.Components.Animations;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.Shields;
using FindTheIdol.Components.Weapons;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace FindTheIdol.Systems.Players
{
    //Get items in in item slots and equip them (from HUD Injector Proxy)
    [UpdateAfter(typeof(PlayerUpdateAnchorsSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class PlayerEquipToAnchorSystem : SystemBase
    {
        private EntityQuery playerQuery;

        protected override void OnCreate()
        {
            this.playerQuery = this.GetEntityQuery(this.EntityManager.GetPlayerIdentifier());
        }

        protected override void OnUpdate()
        {
            //Assumes there is exactly 1 player 
            Entity playerEntity = this.playerQuery.GetSingletonEntity();
            ComponentDataFromEntity<PlayerBodyAnchorComponent> playerBodyComponent = this.GetComponentDataFromEntity<PlayerBodyAnchorComponent>(true);

            //Weapon is in left hand
            this.Entities.WithAll<WeaponEquippedTag>().ForEach((ref ItemInfo itemInfo, ref Translation position, ref Rotation rotation) => 
            {                
                switch(itemInfo.PreservedWeaponAttackType)
                {
                    //Keep rotation of weapon while animating for melee weapons.
                    case WeaponAttackType.Melee:
                        rotation.Value = playerBodyComponent[playerEntity].LeftHandAnchorRotation.Value;
                        break;
                    case WeaponAttackType.Ranged:
                        rotation.Value = playerBodyComponent[playerEntity].LeftHandAnchorRotation.Value;
                        break;
                    //Keep position while rotating for the magic attack animation
                    case WeaponAttackType.Magic:
                        position.Value = playerBodyComponent[playerEntity].LeftHandAnchorPosition.Value;
                        break;
                }
            }).WithReadOnly(playerBodyComponent).Run();

            this.Entities.WithNone<CurrentlyAnimating>().WithAll<WeaponEquippedTag>().ForEach((ref Translation position, ref Rotation rotation) => 
            {
                position.Value = playerBodyComponent[playerEntity].LeftHandAnchorPosition.Value;
                rotation.Value = playerBodyComponent[playerEntity].LeftHandAnchorRotation.Value;
            }).WithReadOnly(playerBodyComponent).Run();

            //Shield is in right hand
            this.Entities.WithAll<ShieldEquippedTag>().ForEach((ref Rotation rotation) => 
            {
                rotation.Value = playerBodyComponent[playerEntity].RightHandAnchorRotation.Value;
            }).WithReadOnly(playerBodyComponent).Run();

            this.Entities.WithNone<CurrentlyAnimating>().WithAll<ShieldEquippedTag>().ForEach((ref Translation position) => 
            {
                position.Value = playerBodyComponent[playerEntity].RightHandAnchorPosition.Value;
            }).WithReadOnly(playerBodyComponent).Run();
        }
    }
}