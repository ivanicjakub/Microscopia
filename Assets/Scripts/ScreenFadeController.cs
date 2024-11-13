namespace Secrets.Gameplay
{
    using UnityEngine;
    using System.Threading.Tasks;
    public class ScreenFadeController : MonoBehaviour
    {
        public static ScreenFadeController Instance;

        [SerializeField] private string fadePropertyName = "_FadeValue";
        private float currentFadeValue;
        private float targetFadeValue;
        private float fadeStartTime;
        private float fadeDuration;
        private bool isFading;

        public bool IsFading => isFading;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                currentFadeValue = 0f;
                Shader.SetGlobalFloat(fadePropertyName, currentFadeValue);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if(Instance == this)
                Instance = null;
        }

        private void Update()
        {
            if (!isFading) return;

            float timeSinceStart = Time.time - fadeStartTime;
            float progress = timeSinceStart / fadeDuration;

            if (progress >= 1f)
            {
                currentFadeValue = targetFadeValue;
                isFading = false;
            }
            else
            {
                currentFadeValue = Mathf.Lerp(currentFadeValue, targetFadeValue, progress);
            }

            Shader.SetGlobalFloat(fadePropertyName, currentFadeValue);
        }

        public async Task FadeIn(float duration)
        {
            await Fade(0f, duration);
        }

        public async Task FadeOut(float duration)
        {
            await Fade(1f, duration);
        }

        public async Task Fade(float targetValue, float duration)
        {
            fadeStartTime = Time.time;
            fadeDuration = duration;
            targetFadeValue = targetValue;
            isFading = true;

            while (isFading)
            {
                await Task.Yield();
            }
        }

        public void SetInstantFade(float value)
        {
            currentFadeValue = value;
            targetFadeValue = value;
            isFading = false;
            Shader.SetGlobalFloat(fadePropertyName, value);
        }
    }
}