using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
namespace Secrets.Cinematics
{
    [RequireComponent(typeof(PlayableDirector))]
    public class PlayableDirectorActions : MonoBehaviour
    {
        private PlayableDirector director;

        [Header("Events")]
        public UnityEvent onPlay;
        public UnityEvent onPause;
        public UnityEvent onStop;
        public UnityEvent onFinish;

        [Header("Runtime References")]
        [SerializeField]
        private bool playOnStart = false;

        [SerializeField]
        private bool pauseOnDisable = true;

        private void Awake()
        {
            director = GetComponent<PlayableDirector>();
            director.stopped += OnTimelineFinished;
        }

        private void Start()
        {
            if (playOnStart)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            if (pauseOnDisable)
            {
                Pause();
            }
        }

        private void OnDestroy()
        {
            if (director != null)
            {
                director.stopped -= OnTimelineFinished;
            }
        }

        // Play the timeline from current position
        public void Play()
        {
            if (director != null)
            {
                director.Play();
                onPlay?.Invoke();
            }
        }

        // Play the timeline from the beginning
        public void PlayFromStart()
        {
            if (director != null)
            {
                director.time = 0;
                director.Play();
                onPlay?.Invoke();
            }
        }

        // Pause the timeline
        public void Pause()
        {
            if (director != null)
            {
                director.Pause();
                onPause?.Invoke();
            }
        }

        // Stop the timeline and reset to beginning
        public void Stop()
        {
            if (director != null)
            {
                director.Stop();
                onStop?.Invoke();
            }
        }

        // Skip to a specific time (in seconds)
        public void SetTime(float timeInSeconds)
        {
            if (director != null)
            {
                director.time = timeInSeconds;
            }
        }

        // Skip forward by specified seconds
        public void SkipForward(float seconds)
        {
            if (director != null)
            {
                director.time = (double)Mathf.Min((float)director.time + seconds, (float)director.duration);
            }
        }

        // Skip backward by specified seconds
        public void SkipBackward(float seconds)
        {
            if (director != null)
            {
                director.time = (double)Mathf.Max((float)director.time - seconds, 0);
            }
        }

        // Jump to the start of the timeline
        public void JumpToStart()
        {
            SetTime(0);
        }

        // Jump to the end of the timeline
        public void JumpToEnd()
        {
            if (director != null)
            {
                SetTime((float)director.duration);
            }
        }

        // Set playback speed (1 is normal speed)
        public void SetSpeed(float speed)
        {
            if (director != null)
            {
                director.playableGraph.GetRootPlayable(0).SetSpeed(speed);
            }
        }

        // Get current timeline time in seconds
        public float GetCurrentTime()
        {
            return director != null ? (float)director.time : 0f;
        }

        // Get total timeline duration in seconds
        public float GetDuration()
        {
            return director != null ? (float)director.duration : 0f;
        }

        // Check if timeline is currently playing
        public bool IsPlaying()
        {
            return director != null && director.state == PlayState.Playing;
        }

        // Called when timeline reaches the end
        private void OnTimelineFinished(PlayableDirector aDirector)
        {
            if (aDirector == director)
            {
                onFinish?.Invoke();
            }
        }
    }
}