namespace FindTheIdol.MonoBehaviours.GameState
{
    public class GameStateUIInjectorProxy : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField]
        private UnityEngine.GameObject gameWinLabel;

        [UnityEngine.SerializeField]
        private UnityEngine.GameObject gameLostLabel;

        public UnityEngine.UI.Text GameWinText { get; set; }

        public UnityEngine.UI.Text GameLostText { get; set; }

        private void Start() 
        {
            this.GameWinText = this.gameWinLabel.GetComponent<UnityEngine.UI.Text>();
            this.GameLostText = this.gameLostLabel.GetComponent<UnityEngine.UI.Text>();

            //Display this later on
            this.GameWinText.enabled = false;
            this.GameLostText.enabled = false;
        }
    }
}