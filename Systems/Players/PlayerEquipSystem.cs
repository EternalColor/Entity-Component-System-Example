using System.Linq;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Deletion;
using FindTheIdol.Components.Players;
using FindTheIdol.MonoBehaviours.Items;
using FindTheIdol.MonoBehaviours.UI;
using FindTheIdol.Utilities.Physics.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

namespace FindTheIdol.Systems.Players
{
    //Get items in in item slots and equip them (from HUD Injector Proxy)
    [BurstCompile]
    public class PlayerEquipSystem : ComponentSystem
    {
        private HUDInjectorProxy hudInjectorProxy;        
        private ItemCatalog itemCatalog;

        private EntityQuery playerQuery;

        protected override void OnCreate()
        {
            this.hudInjectorProxy = UnityEngine.Object.FindObjectOfType<HUDInjectorProxy>();
            this.itemCatalog = UnityEngine.Object.FindObjectOfType<ItemCatalog>();
            this.playerQuery = this.GetEntityQuery(this.EntityManager.GetPlayerIdentifier());
        }

        protected override void OnUpdate()
        {
            //Prevent this from being null when scene is loaded from another scene (Something about the lifecycle of ECS not behaving well with normal gameobject instantiating -> i can not always guarantee that oncreate receives the gameobject)
            if(this.hudInjectorProxy == null) 
            {
                this.hudInjectorProxy = UnityEngine.Object.FindObjectOfType<HUDInjectorProxy>();
            }

            if(this.itemCatalog == null)
            {
                this.itemCatalog = UnityEngine.Object.FindObjectOfType<ItemCatalog>();
            }

            //Assumes there is exactly 1 player 
            Entity playerEntity = this.playerQuery.GetSingletonEntity();
            PlayerBodyAnchorComponent playerBodyAnchorComponent = this.EntityManager.GetComponentData<PlayerBodyAnchorComponent>(playerEntity);

            //Are there unequipped weapons in weapon slot? If so equip them
            //Can not do FirstOrDefault and then check for null becaue ItemSlotDTO is a struct -> Have to count them first
            if(this.hudInjectorProxy.WeaponSlotCollectionDTO.ItemSlotDTOs.Count(weapon => weapon.HasItem) > 0)
            {
                if(this.hudInjectorProxy.WeaponSlotCollectionDTO.ItemSlotDTOs.Count(weapon => !weapon.IsEquippedToPlayer) > 0)
                {
                    ItemSlotDTO weaponSlot = this.hudInjectorProxy.WeaponSlotCollectionDTO.ItemSlotDTOs.First(weapon => weapon.HasItem && !weapon.IsEquippedToPlayer);
                    Entity weaponEntity = this.EntityManager.CreateEntity(this.EntityManager.CreateEquippedWeaponArchetype());

                    ItemDTO weaponItem = this.itemCatalog.PremadeWeapons.Where(weapon => weapon.ItemInfo.PreservedWeaponType == weaponSlot.OriginalInventoryItem.ItemInfo.PreservedWeaponType).First();

                    //Weapon is always placed in left hand
                    SetComponentDataForEquippableEntity(this.EntityManager, weaponEntity, in weaponItem, in playerBodyAnchorComponent.LeftHandAnchorPosition);

                    weaponSlot.IsEquippedToPlayer = true;

                    //We previously took the first element so element 0 is also the first element
                    this.hudInjectorProxy.WeaponSlotCollectionDTO.ItemSlotDTOs[0] = weaponSlot;
                }
            }
            //Unequip
            else
            {
                NativeArray<Entity> equippedWeapons = this.GetEntityQuery(this.EntityManager.GetEquippedWeaponIdentifier()).ToEntityArray(Allocator.TempJob);

                for(int i = 0; i < equippedWeapons.Length; ++i)
                {
                    this.PostUpdateCommands.AddComponent<DeletionTag>(equippedWeapons[i]);
                }

                equippedWeapons.Dispose();
            }

            //Are there unequipped shields in shield slot? If so equip them
            if(this.hudInjectorProxy.ShieldSlotCollectionDTO.ItemSlotDTOs.Count(shield => shield.HasItem) > 0)
            {
                if(this.hudInjectorProxy.ShieldSlotCollectionDTO.ItemSlotDTOs.Count(shield => !shield.IsEquippedToPlayer) > 0)
                {
                    ItemSlotDTO shieldSlot = this.hudInjectorProxy.ShieldSlotCollectionDTO.ItemSlotDTOs.First(shield => shield.HasItem && !shield.IsEquippedToPlayer);
                    Entity shieldEntity = this.EntityManager.CreateEntity(this.EntityManager.CreateEquippedShieldArchetype());

                    ItemDTO shieldItem = this.itemCatalog.PremadeWeapons.Where(weapon => weapon.ItemInfo.PreservedShieldType == shieldSlot.OriginalInventoryItem.ItemInfo.PreservedShieldType).First();

                    //Weapon is always placed in left hand
                    SetComponentDataForEquippableEntity(this.EntityManager, shieldEntity, in shieldItem, in playerBodyAnchorComponent.RightHandAnchorPosition);

                    shieldSlot.IsEquippedToPlayer = true;

                    //We previously took the first element so element 0 is also the first element
                    this.hudInjectorProxy.ShieldSlotCollectionDTO.ItemSlotDTOs[0] = shieldSlot;
                }
            }
            //Unequip
            else
            {
                NativeArray<Entity> equippedShields = this.GetEntityQuery(this.EntityManager.GetEquippedShieldIdentifier()).ToEntityArray(Allocator.TempJob);

                for(int i = 0; i < equippedShields.Length; ++i)
                {
                    this.PostUpdateCommands.AddComponent<DeletionTag>(equippedShields[i]);
                }

                equippedShields.Dispose();
            }
        }

        private void SetComponentDataForEquippableEntity(EntityManager entityManager, Entity equippableEntity, in ItemDTO itemDTO, in Translation destinationInWorld)
        {
            entityManager.SetComponentData(equippableEntity, itemDTO.ItemInfo);

            entityManager.SetComponentData(equippableEntity, destinationInWorld);

            entityManager.SetComponentData(equippableEntity, new NonUniformScale
            {
                Value = itemDTO.Mesh.bounds.size
            });

            entityManager.SetComponentData(equippableEntity, new PhysicsCollider 
            {
                Value = BoxCollider.Create
                (
                    CollisionGeomeotrySingleton.Instance.CreateOrGetBoxGeometry(itemDTO.Mesh.bounds.size), 
                    CollisionFilterSingleton.Instance.BelongsToWeaponFilter
                )
            });
             
            entityManager.SetSharedComponentData(equippableEntity, new RenderMesh 
            {
                mesh = itemDTO.Mesh,
                material = itemDTO.Material
            });
        }
    }
}