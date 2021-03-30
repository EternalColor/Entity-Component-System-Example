using FindTheIdol.Components.Archetypes;
using FindTheIdol.Components.GameState;
using FindTheIdol.MonoBehaviours;
using FindTheIdol.MonoBehaviours.GameState;
using Unity.Burst;
using Unity.Entities;

namespace FindTheIdol.Systems.UI
{
    //This Sytem interacts with the UnityEngine.UI hence its single threaded (ComponentSystem)
    [BurstCompile]  
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class GameStateUISystem : ComponentSystem
    {
        private GameStateUIInjectorProxy gameStateUIInjectorProxy;

        private EntityQuery gameStateComponentQuery;
        
        protected override void OnCreate()
        {
            this.gameStateUIInjectorProxy = UnityEngine.Object.FindObjectOfType<GameStateUIInjectorProxy>();
            this.gameStateComponentQuery = this.GetEntityQuery(this.EntityManager.GetGameStateIdentifier());
        }

        protected override void OnUpdate()
        {
            Entity gameStateEntity = this.gameStateComponentQuery.GetSingletonEntity();
            GameStateComponent gameStateComponent = this.EntityManager.GetComponentData<GameStateComponent>(gameStateEntity);

            //Prevent this from being null when scene is loaded from another scene (Something about the lifecycle of ECS not behaving well with normal gameobject instantiating -> i can not always guarantee that oncreate receives the gameobject)
            if(this.gameStateUIInjectorProxy == null)
            {
                this.gameStateUIInjectorProxy = UnityEngine.Object.FindObjectOfType<GameStateUIInjectorProxy>();
            }

            switch(gameStateComponent.GameStateEnum)
            {
                case GameStateEnum.Menu:
                case GameStateEnum.InGame:
                case GameStateEnum.Pause:
                    break;

                case GameStateEnum.Lost:
                    this.gameStateUIInjectorProxy.GameLostText.enabled = true;
                    break;
                case GameStateEnum.Win:
                    this.gameStateUIInjectorProxy.GameWinText.enabled = true;
                    break;

                default:
                    break;
            }
        }
    }
}
