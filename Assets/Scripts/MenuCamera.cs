namespace Secrets.Menu
{
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    public class MenuCamera : MonoBehaviour
    {
        [Header("Camera Reference")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private UniversalAdditionalCameraData targetCameraData;
        [Header("Field of View Settings")]
        [SerializeField] private float minFOV = 60f;
        [SerializeField] private float maxFOV = 90f;

        [Header("Timing Settings")]
        [SerializeField] private float minTimeInterval = 5f;
        [SerializeField] private float maxTimeInterval = 10f;
        [SerializeField] private float transitionDuration = 2f;

        [Header("Background Color")]
        [SerializeField] Color[] backgroundColors;
        [SerializeField] float backgroundColorChangeInterval = 3f;

        private float currentFOV;
        private float targetFOV;
        private float nextChangeTime;
        private float transitionStartTime;
        private float initialFOV;
        private float _backgroundTimer;
        private int _colorIndex;

        private void Start()
        {
            _backgroundTimer = backgroundColorChangeInterval;
            currentFOV = targetCamera.fieldOfView;
            targetFOV = currentFOV;
            SetNewTargetFOV();
        }

        private void Update()
        {
            if (Input.anyKeyDown)
                _backgroundTimer = -1f;

            if (Time.time >= nextChangeTime)
            {
                SetNewTargetFOV();
            }

            float timeSinceStart = Time.time - transitionStartTime;
            float progress = timeSinceStart / transitionDuration;

            if (progress < 1f)
            {
                currentFOV = Mathf.SmoothStep(initialFOV, targetFOV, progress);
                targetCamera.fieldOfView = currentFOV;
            }

            UpdateBackgroundColor();
        }

        private void SetNewTargetFOV()
        {
            initialFOV = currentFOV;
            targetFOV = Random.Range(minFOV, maxFOV);
            nextChangeTime = Time.time + Random.Range(minTimeInterval, maxTimeInterval);
            transitionStartTime = Time.time;
        }

        private void OnValidate()
        {
            minFOV = Mathf.Max(1f, minFOV);
            maxFOV = Mathf.Min(179f, maxFOV);
            maxFOV = Mathf.Max(minFOV, maxFOV);

            minTimeInterval = Mathf.Max(0.1f, minTimeInterval);
            maxTimeInterval = Mathf.Max(minTimeInterval, maxTimeInterval);
            transitionDuration = Mathf.Max(0.1f, transitionDuration);
        }

        private void UpdateBackgroundColor()
        {
            if (targetCamera.clearFlags != CameraClearFlags.SolidColor)
                return;

            _backgroundTimer -= Time.deltaTime;
            if (_backgroundTimer < 0f)
            {
                _colorIndex++;

                if (_colorIndex >= backgroundColors.Length)
                    _colorIndex = 0;

                targetCamera.backgroundColor = backgroundColors[_colorIndex];

                _backgroundTimer = backgroundColorChangeInterval;
            }
        }
    }
}