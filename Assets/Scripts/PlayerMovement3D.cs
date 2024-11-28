using Secrets.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Secrets.Player
{
    public class PlayerMovement3D : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float movementSmoothness = 0.1f;
        [SerializeField] private float rotationSmoothness = 0.1f;
        [SerializeField] ParticleSystem moveParticles;

        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float cameraSensitivity = 2f;
        [SerializeField] private float minVerticalAngle = -80f;
        [SerializeField] private float maxVerticalAngle = 80f;

        [SerializeField] UnityEvent onMouseDown;
        [SerializeField] SmoothRandomRotator meshRotator;

        // Internal variables
        private Vector3 currentVelocity;
        private float currentRotationVelocity;
        private float cameraPitch = 0f;
        private Vector3 targetMovement;
        private float targetRotation;
        private Vector3 targetLookPosition;
        private Vector3 prevTargetLookPosition;

        private void Start()
        {
            UnlockCursor();

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
            UpdateMoveParticles();
        }

        private void UpdateMoveParticles()
        {
            float mouseX = Input.GetAxis("Mouse X") * (Input.GetMouseButton(1) ? 1f : 0f);
            float horizontal = Mathf.Clamp(Input.GetAxisRaw("Horizontal") + mouseX, -1f, 1f);
            float vertical = Input.GetAxisRaw("Vertical");
            var vol = moveParticles.velocityOverLifetime;
            vol.orbitalY = -4f * horizontal;
            vol.orbitalX = 4f * vertical;
        }

        private void HandleRotation()
        {
            if (Input.GetMouseButtonDown(1))
            {
                LockCursor();
            }

            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

                cameraPitch = Mathf.Clamp(cameraPitch - mouseY, minVerticalAngle, maxVerticalAngle);
                //cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

                targetRotation += mouseX;
                float smoothedRotation = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    targetRotation,
                    ref currentRotationVelocity,
                    rotationSmoothness
                );
                transform.rotation = Quaternion.Euler(cameraPitch, smoothedRotation, 0);
            }

            if (Input.GetMouseButtonUp(1))
            {
                UnlockCursor();
            }
        }

        public void OnMouseDown()
        {
            onMouseDown?.Invoke();
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 targetLookPosition = Vector3.zero;
            targetLookPosition += transform.right * horizontal;
            targetLookPosition += transform.forward * vertical;
            if (targetLookPosition.magnitude > 0.5f && (targetLookPosition - prevTargetLookPosition).magnitude > 0.5f)
            {
                prevTargetLookPosition = targetLookPosition;
                meshRotator.LookAt(transform.position + targetLookPosition, 1.5f);
            }

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            // Zero out y components to keep movement horizontal
            //forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            targetMovement = (forward * vertical + right * horizontal).normalized * moveSpeed;

            Vector3 smoothedMovement = Vector3.SmoothDamp(
                transform.position,
                transform.position + targetMovement,
                ref currentVelocity,
                movementSmoothness
            );

            transform.position = smoothedMovement;
        }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}