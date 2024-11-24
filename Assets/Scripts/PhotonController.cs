using UnityEngine;
namespace Secrets.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhotonController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float baseSpeed = 10f;
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float acceleration = 15f;
        [SerializeField] private float bounceForce = 15f;
        [SerializeField] private float rotationSpeed = 360f;
        [field: SerializeField] private float AutoMoveForward { get; set; } = 0f;
        [SerializeField] private float autoMoveForwardDefaultSpeed = 1f;

        [Header("Quantum State")]
        [SerializeField] private float quantumSpeedMultiplier = 1.5f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color quantumColor = new Color(0.5f, 0f, 1f, 1f);

        [Header("Light Burst")]
        [SerializeField] private float burstRadius = 5f;
        [SerializeField] private float burstForce = 10f;
        [SerializeField] private float burstCooldown = 0.5f;

        [Header("Visual Effects")]
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private ParticleSystem movementParticles;
        [SerializeField] private ParticleSystem burstParticles;
        [SerializeField] private float trailBaseWidth = 0.2f;
        [SerializeField] private float trailMaxWidth = 0.5f;

        [Header("Audio")]
        [SerializeField] private AudioClip movementSound;
        [SerializeField] private AudioClip burstSound;
        [SerializeField] private AudioClip quantumToggleSound;
        [SerializeField] private AudioClip bounceSound;
        [SerializeField] private float minPitch = 0.8f;
        [SerializeField] private float maxPitch = 1.2f;

        private Rigidbody2D rb;
        [SerializeField] SpriteRenderer spriteRenderer;
        private AudioSource audioSource;
        private Vector2 moveInput;
        private float currentSpeed;
        private bool isInQuantumState;
        private float lastBurstTime;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f; // 2D sound

            InitializePhoton();
        }

        private void InitializePhoton()
        {
            currentSpeed = baseSpeed;
            isInQuantumState = false;

            // Set up trail
            if (trailRenderer)
            {
                trailRenderer.startColor = normalColor;
                trailRenderer.endColor = new Color(normalColor.r, normalColor.g, normalColor.b, 0f);
                trailRenderer.startWidth = trailBaseWidth;
            }

            // Set up particles
            if (movementParticles)
            {
                var main = movementParticles.main;
                main.startColor = normalColor;
            }
        }

        private void Update()
        {
            HandleInput();
            UpdateVisualEffects();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleInput()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            // Quantum state toggle
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleQuantumState();
            }

            // Light burst
            if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastBurstTime + burstCooldown)
            {
                FireLightBurst();
            }
        }

        private void HandleMovement()
        {
            if (moveInput != Vector2.zero)
            {
                // Calculate target velocity
                currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);
                Vector2 targetVelocity = moveInput * currentSpeed;

                // Apply movement
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.5f);

                // Rotate towards movement direction
                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.Euler(0, 0, angle),
                    rotationSpeed * Time.fixedDeltaTime
                );

                // Play movement sound
                if (!audioSource.isPlaying && movementSound)
                {
                    audioSource.clip = movementSound;
                    audioSource.pitch = Random.Range(minPitch, maxPitch);
                    audioSource.Play();
                }
            }
            else
            {
                // Decelerate
                currentSpeed = Mathf.Max(currentSpeed - acceleration * Time.fixedDeltaTime, baseSpeed);
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.1f);

                // Stop movement sound
                if (audioSource.clip == movementSound)
                {
                    audioSource.Stop();
                }
            }
        }

        private void UpdateVisualEffects()
        {
            // Update trail based on speed
            if (trailRenderer)
            {
                trailRenderer.startWidth = Mathf.Lerp(trailBaseWidth, trailMaxWidth, rb.linearVelocity.magnitude / maxSpeed);
            }

            // Update movement particles
            if (movementParticles)
            {
                var emission = movementParticles.emission;
                emission.rateOverTime = rb.linearVelocity.magnitude * 2;
            }
        }

        private void ToggleQuantumState()
        {
            isInQuantumState = !isInQuantumState;

            // Visual updates
            Color targetColor = isInQuantumState ? quantumColor : normalColor;
            spriteRenderer.color = targetColor;

            if (trailRenderer)
            {
                trailRenderer.startColor = targetColor;
                trailRenderer.time = isInQuantumState ? 0.5f : 0.2f;
            }

            // Physics updates
            maxSpeed = isInQuantumState ? baseSpeed * quantumSpeedMultiplier : baseSpeed;

            // Play sound
            if (quantumToggleSound)
            {
                audioSource.PlayOneShot(quantumToggleSound);
            }
        }

        private void FireLightBurst()
        {
            lastBurstTime = Time.time;

            // Visual effect
            if (burstParticles)
            {
                burstParticles.Play();
            }

            // Play sound
            if (burstSound)
            {
                audioSource.PlayOneShot(burstSound);
            }

            // Apply force to nearby objects
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, burstRadius);
            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider.gameObject != gameObject)
                {
                    Rigidbody2D hitRb = hitCollider.GetComponent<Rigidbody2D>();
                    if (hitRb != null)
                    {
                        Vector2 direction = (hitCollider.transform.position - transform.position).normalized;
                        hitRb.AddForce(direction * burstForce, ForceMode2D.Impulse);
                    }
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                // Bounce effect
                Vector2 reflection = Vector2.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
                rb.linearVelocity = reflection * Mathf.Min(rb.linearVelocity.magnitude * bounceForce, maxSpeed);

                // Play bounce sound
                if (bounceSound)
                {
                    audioSource.pitch = Random.Range(minPitch, maxPitch);
                    audioSource.PlayOneShot(bounceSound);
                }
            }
        }
    }
}