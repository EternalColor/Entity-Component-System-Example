using System.Collections.Generic;
using System.Linq;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.UI;
using FindTheIdol.MonoBehaviours;
using FindTheIdol.MonoBehaviours.Items;
using FindTheIdol.MonoBehaviours.UI;
using FindTheIdol.Utilities.Constants;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace FindTheIdol.Systems.UI
{
    //This Sytem interacts with the UnityEngine.UI hence its single threaded (ComponentSystem)
    [BurstCompile]   
    [UpdateInGroup(typeof(PresentationSystemGroup))] 
    public class HUDSystem : ComponentSystem
    {
        private EntityQuery playerQuery;
        private HUDInjectorProxy hudInjectorProxy;
        
        protected override void OnCreate()
        {
            this.playerQuery = this.GetEntityQuery(this.EntityManager.GetPlayerIdentifier());
            this.hudInjectorProxy = UnityEngine.Object.FindObjectOfType<HUDInjectorProxy>();
        }

        protected override void OnUpdate()
        {
            Entity player = this.playerQuery.GetSingletonEntity();
            PlayerInfo playerInfo = this.EntityManager.GetComponentData<PlayerInfo>(player);

            DynamicBuffer<InventoryItem> inventoryItemsFromPlayer = this.GetBufferFromEntity<InventoryItem>()[player];

            //Prevent this from being null when scene is loaded from another scene (Something about the lifecycle of ECS not behaving well with normal gameobject instantiating -> i can not always guarantee that oncreate receives the gameobject)
            if(this.hudInjectorProxy == null)
            {
                this.hudInjectorProxy = UnityEngine.Object.FindObjectOfType<HUDInjectorProxy>();
            }

            this.hudInjectorProxy.HealthValueText.text = playerInfo.Health.ToString();
            this.hudInjectorProxy.ManaValueText.text = playerInfo.Mana.ToString();
            this.hudInjectorProxy.LevelValueText.text = playerInfo.Level.ToString();

            //Turn on/off inventory/character slots
            this.hudInjectorProxy.InventorySlotHolderGameObject.SetActive(playerInfo.OpenInventoryFromInputData);
            this.hudInjectorProxy.CharacterSlotHolderGameObject.SetActive(playerInfo.OpenInventoryFromInputData);
            
            //will up item slots (the ones always displayed)
            for(int inventoryItemIndex = 0; inventoryItemIndex < inventoryItemsFromPlayer.Length; ++inventoryItemIndex)
            {
                for(int itemIndex = 0; itemIndex < this.hudInjectorProxy.ItemSlotCollectionDTO.ItemSlotDTOs.Count && !inventoryItemsFromPlayer[inventoryItemIndex].AddedToInventory; ++itemIndex)
                {
                    ItemSlotDTO itemSlotDTO = this.hudInjectorProxy.ItemSlotCollectionDTO.ItemSlotDTOs[itemIndex];

                    //Fill up inventory slots left from right
                    if(!itemSlotDTO.HasItem)
                    {
                        InventoryItem inventoryItem = inventoryItemsFromPlayer[inventoryItemIndex];
                        RenderMesh renderMeshFirstItem = this.EntityManager.GetSharedComponentData<RenderMesh>(inventoryItem.SharedRenderMeshComponentIndex);

                        this.hudInjectorProxy.ItemSlotMeshFilter.mesh = renderMeshFirstItem.mesh;
                        this.hudInjectorProxy.ItemSlotMeshFilter.mesh = ScaleItemMesh(this.hudInjectorProxy.ItemSlotMeshFilter.mesh, this.hudInjectorProxy.ItemSlotMeshRendererRectTransform);

                        //Camera is normaly dsiabled and only enable it to take "screenshots" of the items
                        this.hudInjectorProxy.ItemSlotCamera.enabled = true;
                        this.hudInjectorProxy.ItemSlotCamera.targetTexture = RenderTextureCacheSingleton.Instance.GetOrCreateRenderTexture(itemSlotDTO.GetHashCode(), GameRelatedConstants.ItemSlotSize, GameRelatedConstants.ItemSlotSize, GameRelatedConstants.ItemSlotDepth);

                        this.hudInjectorProxy.ItemSlotMeshRenderer.material = renderMeshFirstItem.material;
                        itemSlotDTO.ItemSlotRawImage.texture = this.RenderCameraToTexture(this.hudInjectorProxy.ItemSlotCamera);
                        this.hudInjectorProxy.ItemSlotCamera.enabled = false;
                        
                        itemSlotDTO.HasItem = true; 
                        itemSlotDTO.OriginalInventoryItem = inventoryItemsFromPlayer[inventoryItemIndex];
                        this.hudInjectorProxy.ItemSlotCollectionDTO.ItemSlotDTOs[itemIndex] = itemSlotDTO;   

                        inventoryItem.AddedToInventory = true;
                        inventoryItemsFromPlayer[inventoryItemIndex] = inventoryItem;
                    }
                }
            }
        }

        private UnityEngine.Mesh ScaleItemMesh(UnityEngine.Mesh itemMesh, UnityEngine.RectTransform meshRendererUIRect)
        {
            //UI is on x,y coordinates but mesh still needs scaling in all directions, we will assume that the item slot has a square shape for now.
            //So we take the y axis for z scaling
            //Scale it up to the size of the "cell"
            float3 scaling = new float3(meshRendererUIRect.sizeDelta.x, meshRendererUIRect.sizeDelta.y, meshRendererUIRect.sizeDelta.y) - new float3(itemMesh.bounds.size);

            //This vertices array needs to be copied before modyfying! Otherwise Unity will throw invalid AABB errors because some vertices are already scaled while others are not yet
            UnityEngine.Vector3[] vertices = itemMesh.vertices;

            for(int i = 0; i < vertices.Length; ++i)
            {
                vertices[i].x *= scaling.x;
                vertices[i].y *= scaling.y;
                vertices[i].z *= scaling.z;
            }

            itemMesh.vertices = vertices;
            
            itemMesh.RecalculateBounds();

            return itemMesh;
        }

        private UnityEngine.Texture2D RenderCameraToTexture(UnityEngine.Camera camera)
        {
            // The Render Texture in RenderTexture.active is the one
            // that will be read by ReadPixels.
            UnityEngine.RenderTexture currentRT = UnityEngine.RenderTexture.active;
            UnityEngine.RenderTexture.active = camera.targetTexture;

            // Render the camera's view.
            camera.Render();

            // Make a new texture and read the active Render Texture into it.
            UnityEngine.Texture2D image = new UnityEngine.Texture2D(camera.targetTexture.width, camera.targetTexture.height);
            image.ReadPixels(new UnityEngine.Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
            image.Apply();

            // Replace the original active Render Texture.
            UnityEngine.RenderTexture.active = currentRT;

            return image;
        }
    }
}
