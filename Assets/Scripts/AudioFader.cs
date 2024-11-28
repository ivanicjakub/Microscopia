using UnityEngine;
namespace Secrets.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioFader : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private float fadeInDuration = 1f;
        [SerializeField] private float fadeOutDuration = 1f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private AudioSource audioSource;
        private float originalVolume;
        private bool isInitialized;
        private AudioClip currentClip;
        private float lastCheckedTime;
        private const float CHECK_INTERVAL = 0.1f; // How often to check for clip changes

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            originalVolume = audioSource.volume;
            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized || !audioSource.isPlaying) return;

            // Check for clip changes periodically
            if (Time.time - lastCheckedTime >= CHECK_INTERVAL)
            {
                UpdateCurrentClip();
                lastCheckedTime = Time.time;
            }

            if (currentClip == null) return;

            float currentTime = audioSource.time;
            float clipLength = currentClip.length;

            // Calculate fade multiplier
            float fadeMultiplier = 1f;

            // Handle start fade in
            if (currentTime < fadeInDuration)
            {
                float fadeProgress = currentTime / fadeInDuration;
                fadeMultiplier = fadeCurve.Evaluate(fadeProgress);
            }
            // Handle end fade out
            else if (currentTime > clipLength - fadeOutDuration)
            {
                float fadeProgress = (clipLength - currentTime) / fadeOutDuration;
                fadeMultiplier = fadeCurve.Evaluate(fadeProgress);
            }

            // Apply volume
            audioSource.volume = originalVolume * fadeMultiplier;
        }

        private void UpdateCurrentClip()
        {
            // Get the currently playing clip from the AudioSource
            var playingClip = audioSource.clip;

            // If the clip has changed, update our reference
            if (playingClip != currentClip)
            {
                currentClip = playingClip;
                if (currentClip != null)
                {
                    Debug.Log($"Now playing: {currentClip.name}, Length: {currentClip.length}s");
                }
            }
        }

        // Optional: Method to change fade durations at runtime
        public void SetFadeDurations(float fadeIn, float fadeOut)
        {
            fadeInDuration = Mathf.Max(0, fadeIn);
            fadeOutDuration = Mathf.Max(0, fadeOut);
        }

        // Optional: Method to change fade curve at runtime
        public void SetFadeCurve(AnimationCurve newCurve)
        {
            fadeCurve = newCurve;
        }

        // Optional: Method to update original volume
        public void SetBaseVolume(float newVolume)
        {
            originalVolume = Mathf.Clamp01(newVolume);
            if (!IsInFadeRegion())
            {
                audioSource.volume = originalVolume;
            }
        }

        // Helper method to check if currently in a fade region
        private bool IsInFadeRegion()
        {
            if (currentClip == null) return false;
            float currentTime = audioSource.time;
            float clipLength = currentClip.length;
            return currentTime < fadeInDuration || currentTime > clipLength - fadeOutDuration;
        }

        // Reset volume when disabled
        private void OnDisable()
        {
            if (audioSource != null)
            {
                audioSource.volume = originalVolume;
            }
        }
    }
}