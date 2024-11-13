namespace Secrets.Gameplay
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ZoomThresholdEvent
    {
        public float scaleThreshold;
        public UnityEvent onThresholdReached;
        public bool hasTriggered;
    }

    public class Zoomable : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [SerializeField] private float minScale = 0.1f;
        [SerializeField] private float maxScale = 10f;
        [SerializeField] private float zoomSpeed = 0.1f;
        [SerializeField] private bool smoothZoom = true;
        [SerializeField] private float smoothSpeed = 10f;

        [Header("Threshold Events")]
        [SerializeField] private List<ZoomThresholdEvent> thresholdEvents = new List<ZoomThresholdEvent>();

        private Camera mainCamera;
        private Vector3 targetScale;
        private Vector3 lastMousePosition;
        private bool isDragging;

        private void Start()
        {
            mainCamera = Camera.main;
            targetScale = transform.localScale;

            // Sort threshold events by scale for consistency
            thresholdEvents.Sort((a, b) => a.scaleThreshold.CompareTo(b.scaleThreshold));
        }

        private void Update()
        {
            HandleZoom();
            HandleDrag();
            UpdateScale();
            CheckThresholds();
        }

        private void HandleZoom()
        {
            float scrollDelta = Input.mouseScrollDelta.y;

            if (scrollDelta != 0)
            {
                // Get mouse position in world space before zoom
                Vector3 mouseWorldPosBefore = mainCamera.ScreenToWorldPoint(Input.mousePosition);

                // Calculate new scale
                float scaleFactor = 1f + (scrollDelta * zoomSpeed);
                Vector3 newScale = targetScale * scaleFactor;

                // Clamp the scale
                newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
                newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
                newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);

                targetScale = newScale;

                // Get mouse position in world space after zoom
                Vector3 mouseWorldPosAfter = mainCamera.ScreenToWorldPoint(Input.mousePosition);

                // Adjust position to zoom towards mouse
                Vector3 offset = mouseWorldPosBefore - mouseWorldPosAfter;
                transform.position += offset;
            }
        }

        private void HandleDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                Vector3 worldDelta = mainCamera.ScreenToWorldPoint(delta) - mainCamera.ScreenToWorldPoint(Vector3.zero);
                transform.position += worldDelta;
                lastMousePosition = Input.mousePosition;
            }
        }

        private void UpdateScale()
        {
            if (smoothZoom)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
            }
            else
            {
                transform.localScale = targetScale;
            }
        }

        private void CheckThresholds()
        {
            float currentScale = transform.localScale.x;  // Assuming uniform scaling

            foreach (var threshold in thresholdEvents)
            {
                if (currentScale >= threshold.scaleThreshold)
                {
                    if (!threshold.hasTriggered)
                    {
                        threshold.onThresholdReached?.Invoke();
                        threshold.hasTriggered = true;
                    }
                }
                else
                {
                    // Reset trigger when scale goes below threshold to allow repeated triggering
                    threshold.hasTriggered = false;
                }
            }
        }

        // Editor helper method to add a new threshold event
        public void AddThresholdEvent(float scale, UnityEvent thresholdEvent)
        {
            thresholdEvents.Add(new ZoomThresholdEvent
            {
                scaleThreshold = scale,
                onThresholdReached = thresholdEvent,
                hasTriggered = false
            });

            // Keep the list sorted
            thresholdEvents.Sort((a, b) => a.scaleThreshold.CompareTo(b.scaleThreshold));
        }
    }
}