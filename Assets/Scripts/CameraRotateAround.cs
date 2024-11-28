using UnityEngine;
using UnityEngine.Events;
namespace Secrets.Gameplay
{
    public class CameraRotateAround : MonoBehaviour
    {
        [SerializeField] private Transform target;        // The target to rotate around
        [SerializeField] private float rotationSpeed = 5f;  // Speed of rotation
        [SerializeField] private UnityEvent onMouseUp;

        private Vector3 initialRotation;
        private Vector3 originalPosition;
        private float currentAngle;
        private float distanceToTarget;

        private void OnEnable()
        {
            // Store initial values when the script is enabled
            initialRotation = transform.eulerAngles;
            originalPosition = transform.position;

            if (target != null)
            {
                // Calculate initial distance to maintain during rotation
                distanceToTarget = Vector3.Distance(transform.position, target.position);
            }
            else
            {
                Debug.LogWarning("No target assigned to CameraRotateAround script!");
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
                onMouseUp?.Invoke();

            if (target == null) return;

            // Check for left mouse button hold
            if (Input.GetMouseButton(0))
            {
                // Get mouse X movement
                float mouseX = Input.GetAxis("Mouse X");

                // Update the angle based on mouse movement
                currentAngle += mouseX * rotationSpeed;

                // Calculate new position
                float radians = currentAngle * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(
                    Mathf.Sin(radians) * distanceToTarget,
                    0,
                    Mathf.Cos(radians) * distanceToTarget
                );
                Vector3 fin = target.position + offset;
                fin.y = originalPosition.y;
                transform.position = fin;
                transform.LookAt(target);
                Vector3 eulers = transform.eulerAngles;
                eulers.x = initialRotation.x;
                transform.eulerAngles = eulers;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                distanceToTarget = Vector3.Distance(transform.position, target.position);
            }
        }
    }
}