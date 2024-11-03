namespace Secrets.Gameplay
{
    using UnityEngine;
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] ZoomController zoomController;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // + Progress visualizer
        // + Timer
        // + Minigames system
        // + Inventory
    }
}