using System.Linq;
using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.UI;
using FindTheIdol.Utilities.Constants;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FindTheIdol.Systems.UI
{
    //This Sytem interacts with the UnityEngine.UI hence its single threaded (ComponentSystem)
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class UIItemPickupSystem : ComponentSystem
    {
        private EntityQuery playerQuery;

        protected override void OnCreate()
        {
            this.playerQuery = this.GetEntityQuery(this.EntityManager.GetSpacialTypes().Concat(new ComponentType[] { this.EntityManager.GetPlayerIdentifier() }).ToArray());
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            //Get pickupable items and spacial and scaling types so we can render the UI worldspace label
            NativeArray<Entity> pickupableItems = this.GetEntityQuery(this.EntityManager.GetSpacialTypes()
                                                    .Concat(new ComponentType[] { this.EntityManager.GetPickupableItemIdentifierFlat() }).ToArray())
                                                    .ToEntityArray(Allocator.TempJob);

            Entity playerEntity = this.playerQuery.GetSingletonEntity();

            this.Entities.WithAll<ItemInfoWorldSpaceLabelInjectorTag>().ForEach((UnityEngine.UI.Text pickupItemText) => 
            {
                //If there are pickupable items around, display text
                bool displayPickupableItem = pickupableItems.Length > 0;
                pickupItemText.enabled = displayPickupableItem;
                if(displayPickupableItem)
                {
                    //TODO: Render multiple labels, for now just for [0] element
                    Translation pickupableItemTranslation = this.EntityManager.GetComponentData<Translation>(pickupableItems[0]);
                    NonUniformScale pickupableItemNonUniformScale = this.EntityManager.GetComponentData<NonUniformScale>(pickupableItems[0]);

                    Translation playerTranslation = this.EntityManager.GetComponentData<Translation>(playerEntity);

                    pickupItemText.transform.position = pickupableItemTranslation.Value + pickupableItemNonUniformScale.Value;

                    //Implement properly
                    /*float3 target = new float3(playerTranslation.Value.x, 0, playerTranslation.Value.z); 
                    float3 direction = math.normalizesafe(target - new float3(pickupItemText.transform.position));

                    //Rotate label towards player.
                    pickupItemText.transform.rotation = math.slerp
                    (
                        pickupItemText.transform.rotation, 
                        quaternion.LookRotationSafe(new float3(direction.x, 0, direction.z), UnityEngine.Vector3.up),
                        GameRelatedConstants.WorldSpaceLabelRotationSpeed * deltaTime
                    );*/
                }
            });

            pickupableItems.Dispose();
        }
    }
}
