namespace Secrets.Gameplay
{
    using UnityEngine;
    using UnityEngine.Events;
    public class HotspotInteraction : MonoBehaviour
    {
        public UnityEvent<bool> onHighlighted;

        private Hotspot _data;
        private ZoomController _controller;

        public void Initialize(Hotspot hotspotData, ZoomController zoomController)
        {
            _data = hotspotData;
            _controller = zoomController;
        }

        private void OnMouseEnter()
        {
            onHighlighted?.Invoke(true);
        }

        private void OnMouseExit()
        {
            onHighlighted?.Invoke(false);
        }

        private void OnMouseDown()
        {
            if (_data == null)
                return;

            switch (_data.interactionType)
            {
                case Hotspot.Type.LoadNextLevel:
                    _controller.TransitionToLevel(_data.nextLevelIndex);
                    onHighlighted?.Invoke(false);
                    enabled = false;
                    break;
                case Hotspot.Type.Event:
                    _data.interactionEvent?.Invoke();
                    break;
                default:
                    break;
            }
        }
    }
}