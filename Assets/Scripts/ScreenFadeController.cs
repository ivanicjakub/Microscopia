namespace Secrets.Gameplay
{
    using UnityEngine;
    using System.Threading.Tasks;
    using UnityEngine.UI;

    public class ScreenFadeController : MonoBehaviour
    {
        public static ScreenFadeController Instance;

        [SerializeField] private Image fadeImage;

        private float _currentFadeValue;
        private float _targetFadeValue;
        private float _fadeStartTime;
        private float _fadeDuration;
        private bool _isFading;

        public static bool IsFading => Instance ? Instance._isFading : false;

        private Color _black;
        private Color _blackTransparent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _currentFadeValue = 0f;
                _black = Color.black;
                _blackTransparent = _black;
                _blackTransparent.a = 0f;
                UpdateGraphicsFadeValue(_currentFadeValue);
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
            if (!_isFading) return;

            float timeSinceStart = Time.time - _fadeStartTime;
            float progress = timeSinceStart / _fadeDuration;

            if (progress >= 1f)
            {
                _currentFadeValue = _targetFadeValue;
                _isFading = false;
            }
            else
            {
                _currentFadeValue = Mathf.Lerp(_currentFadeValue, _targetFadeValue, progress);
            }

            UpdateGraphicsFadeValue(_currentFadeValue);
        }

        private void UpdateGraphicsFadeValue(float value)
        {
            fadeImage.color = Color.Lerp(_blackTransparent, _black, value);
            bool imageShouldBeActive = value > 0.01f;
            if (fadeImage.enabled != imageShouldBeActive)
                fadeImage.enabled = imageShouldBeActive;
        }

        public async void BlackScreenFadeIn()
        {
             await FadeIn(1f);
        }

        public async void BlackScreenFadeOut()
        {
            await FadeOut(2f);
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
            _fadeStartTime = Time.time;
            _fadeDuration = duration;
            _targetFadeValue = targetValue;
            _isFading = true;

            while (_isFading)
            {
                await Task.Yield();
            }
        }

        public void SetInstantFade(float value)
        {
            _currentFadeValue = value;
            _targetFadeValue = value;
            _isFading = false;
            UpdateGraphicsFadeValue(value);
        }
    }
}