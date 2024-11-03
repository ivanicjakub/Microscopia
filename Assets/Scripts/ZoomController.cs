namespace Secrets.Gameplay
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;
    public class ZoomController : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [SerializeField] float minZoom     = 1f;
        [SerializeField] float maxZoom     = 5f;
        [SerializeField] float zoomSpeed   = 1f;
        [SerializeField] float smoothSpeed = 5f;

        [Header("Level Settings")]
        [SerializeField] List<LevelData> levels;
        [SerializeField] SpriteRenderer  levelRenderer;
        [SerializeField] Transform       hotspotContainer;
        [SerializeField] GameObject      hotspotPrefab;

        private Camera           _mainCamera;
        private int              _currentLevelIndex = 0;
        private float            _currentZoom       = 1f;
        private Vector3          _dragOrigin;
        private bool             _isDragging        = false;
        private List<GameObject> _activeHotspots    = new List<GameObject>();

        private void Start()
        {
            _currentZoom = maxZoom;
            _mainCamera = Camera.main;
            LoadLevel(_currentLevelIndex);
        }

        private void Update()
        {
            HandleInput();
            UpdateCameraZoom();
            ClampCameraPosition();
        }

        private void HandleInput()
        {
            float scrollDelta = Input.mouseScrollDelta.y;
            if (scrollDelta != 0)
            {
                float targetZoom = _currentZoom - (scrollDelta * zoomSpeed);
                _currentZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _dragOrigin = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                _isDragging = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }

            if (_isDragging)
            {
                Vector3 difference = _dragOrigin - _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                _mainCamera.transform.position += difference;
            }
        }

        private void UpdateCameraZoom()
        {
            _mainCamera.orthographicSize = Mathf.Lerp( _mainCamera.orthographicSize, _currentZoom, Time.deltaTime * smoothSpeed);
        }

        private void ClampCameraPosition()
        {
            if (levelRenderer == null) return;

            float camHeight = _mainCamera.orthographicSize;
            float camWidth = camHeight * _mainCamera.aspect;

            Bounds bounds = levelRenderer.bounds;
            float minX = bounds.min.x + camWidth;
            float maxX = bounds.max.x - camWidth;
            float minY = bounds.min.y + camHeight;
            float maxY = bounds.max.y - camHeight;

            Vector3 pos = _mainCamera.transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            _mainCamera.transform.position = pos;
        }

        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= levels.Count) return;

            foreach (var hotspot in _activeHotspots)
                Destroy(hotspot);

            _activeHotspots.Clear();

            _currentLevelIndex = levelIndex;
            LevelData levelData = levels[levelIndex];
            levelRenderer.sprite = levelData.levelImage;

            foreach (var hotspotData in levelData.hotspots)
                CreateHotspot(hotspotData);

            ResetCamera();
        }

        private void CreateHotspot(Hotspot hotspotData)
        {
            GameObject hotspot = Instantiate(hotspotPrefab, hotspotContainer);
            hotspot.transform.localPosition = hotspotData.position;

            HotspotInteraction interaction = hotspot.GetComponent<HotspotInteraction>();
            if (interaction != null)
            {
                interaction.Initialize(hotspotData, this);
            }

            _activeHotspots.Add(hotspot);
        }

        private void ResetCamera()
        {
            _currentZoom = 1f;
            _mainCamera.transform.position = new Vector3(0, 0, -10);
            _mainCamera.orthographicSize = 5f;
        }

        public void TransitionToLevel(int nextLevelIndex)
        {
            //TODO Animation
            LoadLevel(nextLevelIndex);
        }
    }

    [Serializable]
    public class Hotspot
    {
        public enum Type { LoadNextLevel = 0, Event = 1 };
        public Vector2 position;
        public int nextLevelIndex;
        public Type interactionType = Type.LoadNextLevel;
        public UnityEvent interactionEvent;
    }
}