using UnityEngine;
using System.Collections;
namespace Secrets.Gameplay
{
    public class SmoothRandomRotator : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Tooltip("How fast the object rotates towards its target rotation")]
        [SerializeField] private float rotationSpeed = 2.0f;

        [Tooltip("How long to wait between starting new rotations")]
        [SerializeField] private float intervalBetweenRotations = 3.0f;

        [Header("Rotation Limits")]
        [Tooltip("Maximum rotation angle per axis (in degrees)")]
        [SerializeField] private Vector3 maxRotationAngles = new Vector3(360f, 360f, 360f);

        private Quaternion targetRotation;
        private bool isRotating = false;
        private bool isLookingAt = false;
        private Coroutine randomRotationCoroutine;
        private Coroutine lookAtCoroutine;

        private void Start()
        {
            // Start the rotation cycle
            StartRandomRotation();
        }

        private void Update()
        {
            if (isRotating && !isLookingAt)
            {
                // Smoothly interpolate towards the target rotation
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                // Check if we're close enough to the target to stop rotating
                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
                {
                    isRotating = false;
                }
            }
        }

        private void StartRandomRotation()
        {
            if (randomRotationCoroutine != null)
            {
                StopCoroutine(randomRotationCoroutine);
            }
            randomRotationCoroutine = StartCoroutine(RotationCycle());
        }

        private IEnumerator RotationCycle()
        {
            while (true)
            {
                float horizontal = Input.GetAxisRaw("Horizontal");
                float vertical = Input.GetAxisRaw("Vertical");

                if (!isLookingAt && (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f)))
                {
                    // Generate random rotation angles within the specified limits
                    Vector3 randomRotation = new Vector3(
                        Random.Range(-maxRotationAngles.x, maxRotationAngles.x),
                        Random.Range(-maxRotationAngles.y, maxRotationAngles.y),
                        Random.Range(-maxRotationAngles.z, maxRotationAngles.z)
                    );

                    // Set the new target rotation
                    targetRotation = Quaternion.Euler(randomRotation);
                    isRotating = true;

                    // Wait until the current rotation is complete
                    yield return new WaitUntil(() => !isRotating);

                    // Wait for the specified interval before starting the next rotation
                    yield return new WaitForSeconds(intervalBetweenRotations);
                }
                else
                {
                    yield return null;
                }
            }
        }

        public void LookAt(Transform target)
        {
            LookAt(target.position);
        }

        public void LookAt(Vector3 targetPosition, float duration = 1f)
        {
            if (lookAtCoroutine != null)
            {
                StopCoroutine(lookAtCoroutine);
            }
            lookAtCoroutine = StartCoroutine(LookAtCoroutine(targetPosition, duration));
        }

        private IEnumerator LookAtCoroutine(Vector3 targetPosition, float duration)
        {
            isLookingAt = true;
            isRotating = false;

            Quaternion startRotation = transform.rotation;
            Quaternion targetLookRotation = Quaternion.LookRotation(targetPosition - transform.position);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Use smoothstep interpolation for more natural motion
                //float smoothT = t * t * (3f - 2f * t);

                transform.rotation = Quaternion.Slerp(startRotation, targetLookRotation, t);
                yield return null;
            }

            transform.rotation = targetLookRotation;
            isLookingAt = false;

            // Resume random rotation
            StartRandomRotation();
        }

        // Optional: Public methods to control rotation behavior at runtime
        public void SetRotationSpeed(float speed)
        {
            rotationSpeed = Mathf.Max(0, speed);
        }

        public void SetInterval(float interval)
        {
            intervalBetweenRotations = Mathf.Max(0, interval);
        }

        public void SetMaxRotationAngles(Vector3 angles)
        {
            maxRotationAngles = angles;
        }
    }
}