using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FindTheIdol.Components.Items;
using FindTheIdol.Utilities.Constants;
using FindTheIdol.Utilities.Cursor;
using UnityEngine;

namespace FindTheIdol.MonoBehaviours.Items
{
    public class ItemSlotButtonHandler : UnityEngine.MonoBehaviour
    {
        private Dictionary<ItemSlotType, ItemSlotCollectionDTO> itemSlotCollectionDTOs;

        public ItemSlotButtonHandler()
        {
            this.itemSlotCollectionDTOs = new Dictionary<ItemSlotType, ItemSlotCollectionDTO>();
        }

        public void RegisterItemSlotCollectionDTO(ItemSlotType itemSlotType, ItemSlotCollectionDTO itemSlotCollectionDTO)
        {
            this.itemSlotCollectionDTOs.Add(itemSlotType, itemSlotCollectionDTO);
            itemSlotCollectionDTO.ItemSlotDTOs.ForEach(itemSlotDTO => this.AddOnClickDelegateFunction(itemSlotDTO, itemSlotType));
        }

        public void UnRegisterAllItemSlotCollectionDTO()
        {
            foreach(KeyValuePair<ItemSlotType, ItemSlotCollectionDTO> keyValuePairSlotCollection in itemSlotCollectionDTOs)
            {
                keyValuePairSlotCollection.Value.ItemSlotDTOs
                    .ForEach(itemSlotDTO => this.RemoveOnClickDelegateFunction(itemSlotDTO, keyValuePairSlotCollection.Key));
            }
        }

        private void AddOnClickDelegateFunction(ItemSlotDTO itemSlotDTO, ItemSlotType itemSlotType)
        {
            itemSlotDTO.ItemSlotButton.onClick.AddListener(delegate { ItemSlotOnClickDelegateFunction(itemSlotDTO.Index, itemSlotType); });
        }

        private void RemoveOnClickDelegateFunction(ItemSlotDTO itemSlotDTO, ItemSlotType itemSlotType)
        {
            itemSlotDTO.ItemSlotButton.onClick.RemoveListener(delegate { ItemSlotOnClickDelegateFunction(itemSlotDTO.Index, itemSlotType); });
        }

        private void ItemSlotOnClickDelegateFunction(int index, ItemSlotType itemSlotType)
        {
            ItemSlotDTO itemSlotDTO = this.itemSlotCollectionDTOs[itemSlotType].ItemSlotDTOs[index];

            if(itemSlotDTO.HasItem && itemSlotDTO.ItemSlotRawImage.texture != null)
            {
                //The cursor has no texture yet, but the source has it
                if(CursorInputWrapper.CursorItemTexture == null)
                {   
                    CursorInputWrapper.CursorItemTexture = itemSlotDTO.ItemSlotRawImage.texture;
                    CursorInputWrapper.OriginalInventoryItem = itemSlotDTO.OriginalInventoryItem;

                    //Clear out the item slot texture after it was clicked
                    itemSlotDTO.ItemSlotRawImage.texture = null;
                    itemSlotDTO.HasItem = false;
                    itemSlotDTO.OriginalInventoryItem = default;
                }
                //The source and destination both have a texture
                else if(CanBePlacedInSlot(itemSlotType))
                {
                    //Swap the textures if target has a texture (target is itemSlotDto in this case)
                    Texture tempTexture = itemSlotDTO.ItemSlotRawImage.texture;
                    InventoryItem tempInventoryItem = itemSlotDTO.OriginalInventoryItem;
                    
                    itemSlotDTO.ItemSlotRawImage.texture = CursorInputWrapper.CursorItemTexture;
                    itemSlotDTO.OriginalInventoryItem = CursorInputWrapper.OriginalInventoryItem;
                    itemSlotDTO.HasItem = true;

                    CursorInputWrapper.CursorItemTexture = tempTexture;
                    CursorInputWrapper.OriginalInventoryItem = tempInventoryItem;
                }
            }
            //Destination has no item yet (cursor already has a texture)
            else if(CursorInputWrapper.CursorItemTexture != null && CanBePlacedInSlot(itemSlotType))
            {
                //Target item slot has no texture so set it and then "remove" the cursor item texture (set to null)
                itemSlotDTO.ItemSlotRawImage.texture = CursorInputWrapper.CursorItemTexture;
                itemSlotDTO.OriginalInventoryItem = CursorInputWrapper.OriginalInventoryItem;
                itemSlotDTO.HasItem = true;
                CursorInputWrapper.CursorItemTexture = null;
            }

            //Force the player equip system to re-evaluate the item slot dto
            itemSlotDTO.IsEquippedToPlayer = false;
            
            this.itemSlotCollectionDTOs[itemSlotType].ItemSlotDTOs[index] = itemSlotDTO;
        }

        private bool CanBePlacedInSlot(ItemSlotType destinationType)
        {
            switch(destinationType)
            {
                //Every item can be placed in the item slot
                case ItemSlotType.Item:
                    return true;
                case ItemSlotType.Weapon:
                    return CursorInputWrapper.OriginalInventoryItem.ItemInfo.IsWeapon;
                case ItemSlotType.Shield:
                    return CursorInputWrapper.OriginalInventoryItem.ItemInfo.IsShield;
                //No default case because we compare against a enum
                //TODO: Add cases
            }
            
            //TODO: Remove
            return false;
        }
    }
}
