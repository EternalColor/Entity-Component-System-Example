using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.GameState;
using Unity.Entities;

namespace FindTheIdol.MonoBehaviours.GameState
{
    [RequiresEntityConversion]
    public class GameStateAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        [UnityEngine.SerializeField]
        private GameStateEnum gameStateEnum;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Entity gameStateEntity = dstManager.CreateEntity(dstManager.CreateGameStateArchetype());

            dstManager.SetComponentData(gameStateEntity, new GameStateComponent { GameStateEnum = gameStateEnum });
        }
    }
}
