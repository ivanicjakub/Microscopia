namespace OravaGames.Events
{
    using UnityEngine;
    using UnityEngine.Events;
    public class DelayedEvent : MonoBehaviour
    {
        public UnityEvent actionEvent;
        public float delay = 0f;

        public void OnEnable()
        {
            if (delay < 0.01f)
                InvokeEvent();
            else
                Invoke(nameof(InvokeEvent), delay);
        }

        public void InvokeEvent()
        {
            actionEvent?.Invoke();
        }
    }
}