using UnityEngine;
namespace Secrets.Player
{
    public class PlayerMovement3D : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float movementSmoothness = 0.1f;
        [SerializeField] private float rotationSmoothness = 0.1f;

        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float cameraSensitivity = 2f;
        [SerializeField] private float minVerticalAngle = -80f;
        [SerializeField] private float maxVerticalAngle = 80f;

        // Internal variables
        private Vector3 currentVelocity;
        private float currentRotationVelocity;
        private float cameraPitch = 0f;
        private Vector3 targetMovement;
        private float targetRotation;

        private void Start()
        {
            // Lock and hide cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // If camera transform not assigned, try to find main camera
            if (cameraTransform == null)
            {
                var mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    cameraTransform = mainCamera.transform;
                }
                else
                {
                    Debug.LogError("No camera assigned and couldn't find main camera!");
                }
            }
        }

        private void Update()
        {
            HandleRotation();
            HandleMovement();
        }

        private void HandleRotation()
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

            // Update camera pitch
            cameraPitch = Mathf.Clamp(cameraPitch - mouseY, minVerticalAngle, maxVerticalAngle);
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

            // Update player rotation (horizontal only)
            targetRotation += mouseX;
            float smoothedRotation = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetRotation,
                ref currentRotationVelocity,
                rotationSmoothness
            );
            transform.rotation = Quaternion.Euler(0, smoothedRotation, 0);
        }

        private void HandleMovement()
        {
            // Get input
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Calculate movement direction relative to camera
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            // Zero out y components to keep movement horizontal
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            // Calculate target movement
            targetMovement = (forward * vertical + right * horizontal).normalized * moveSpeed;

            // Apply smoothing to movement
            Vector3 smoothedMovement = Vector3.SmoothDamp(
                transform.position,
                transform.position + targetMovement,
                ref currentVelocity,
                movementSmoothness
            );

            // Update position
            transform.position = smoothedMovement;
        }

        // Call this when you want to unlock the cursor (e.g., for menus)
        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Call this when you want to relock the cursor (e.g., resuming gameplay)
        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}