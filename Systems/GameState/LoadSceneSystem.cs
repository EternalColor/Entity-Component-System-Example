using FindTheIdol.Components.GameState;
using Unity.Burst;
using Unity.Entities;

namespace FindTheIdol.Systems.GameState
{
    [BurstCompile]
    public class LoadSceneSystem : ComponentSystem
    {
        private const float LostTimerDuration = 3f;

        private bool startToAccumulateTimer = false;

        private float timer = 0f;

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            this.Entities.ForEach((ref GameStateComponent gameStateComponent) =>
            {
                switch(gameStateComponent.GameStateEnum)
                {
                    case GameStateEnum.Lost:
                        //Let the player see that he lost the game, so we delay this
                        if(!startToAccumulateTimer)
                        {
                            startToAccumulateTimer = true;
                        }    

                        if(timer >= LoadSceneSystem.LostTimerDuration)
                        {
                            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                            startToAccumulateTimer = false;
                            EntityManager.DestroyEntity(EntityManager.UniversalQuery);
                        }
                        break;
                    //No default case because we compare to a enum
                }

                if(startToAccumulateTimer)
                {
                    this.timer += deltaTime;
                }
            });
        }
    }
}