using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.Items;
using FindTheIdol.Components.Players;
using FindTheIdol.Components.Weapons;
using FindTheIdol.MonoBehaviours.CharacterCreator;
using FindTheIdol.MonoBehaviours.Items;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace FindTheIdol.MonoBehaviours.Players
{
    [RequiresEntityConversion]
    public class PlayerFromCreatorDTOAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        [UnityEngine.SerializeField]
        private PlayerInfo playerInfoFromInspector;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //Finding the creator dto, taking the data that was "created" in the character creator and override the PlayerInfo fields which needs modification.
            //So we combine the playerInfo from the inspector and from CreatorDTO
            CreatorDTO creatorDTO = UnityEngine.Object.FindObjectOfType<CreatorDTO>();

            creatorDTO.ChosenPlayerInfo.WalkSpeed = this.playerInfoFromInspector.WalkSpeed;
            creatorDTO.ChosenPlayerInfo.ArrowCount = this.playerInfoFromInspector.ArrowCount;
            creatorDTO.ChosenPlayerInfo.RunSpeed = this.playerInfoFromInspector.RunSpeed;
            creatorDTO.ChosenPlayerInfo.JumpForce = this.playerInfoFromInspector.JumpForce;
            creatorDTO.ChosenPlayerInfo.LookSpeed  = this.playerInfoFromInspector.LookSpeed;

            dstManager.AddComponentData(entity, creatorDTO.ChosenPlayerInfo);

            using(NativeArray<Entity> startItemEntities = dstManager.CreateEntity(dstManager.CreateItemDirectlyToInventoryArchetype(), creatorDTO.StartItems.Count, Allocator.Temp))
            {
                for(int i = 0; i < startItemEntities.Length; ++i)
                {
                    dstManager.SetComponentData(startItemEntities[i], creatorDTO.StartItems[i].ItemInfo);
                    
                    dstManager.SetSharedComponentData<RenderMesh>(startItemEntities[i], new RenderMesh
                    {
                        mesh = creatorDTO.StartItems[i].Mesh,
                        material = creatorDTO.StartItems[i].Material
                    });
                }
            }
        }
    }
}