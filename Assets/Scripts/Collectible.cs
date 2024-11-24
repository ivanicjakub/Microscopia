namespace Secrets.Gameplay
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(Collider))]
    public class Collectible : MonoBehaviour
    {
        [Header("Collection Settings")]
        [SerializeField] private string collectorTag = "Player";
        [SerializeField] private bool destroyOnCollect = true;
        [SerializeField] private bool useParticleCollision = false;
        [SerializeField] private bool useTriggerCollision = true;
        [SerializeField] private float destroyDelay = 0f;

        [Header("Visual Feedback")]
        [SerializeField] private ParticleSystem collectEffect;
        [SerializeField] private AudioSource collectSound;

        [Header("Events")]
        public UnityEvent<Collectible> onCollect;
        public UnityEvent<Collectible> onCollectStarted;
        public UnityEvent<Collectible> onCollectFinished;

        private bool isCollected = false;
        private ParticleSystem.CollisionModule collisionModule;
        private Collider[] _colliders;

        private void Awake()
        {
            if (useParticleCollision && GetComponent<ParticleSystem>())
            {
                var particleSystem = GetComponent<ParticleSystem>();
                collisionModule = particleSystem.collision;
                collisionModule.enabled = true;
                collisionModule.sendCollisionMessages = true;
            }

            if (useTriggerCollision)
            {
                _colliders = GetComponents<Collider>();
                foreach (var item in _colliders)
                    item.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!useTriggerCollision || isCollected) return;

            if (other.CompareTag(collectorTag))
            {
                Collect(other.gameObject);
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            if (!useParticleCollision || isCollected) return;

            if (other.CompareTag(collectorTag))
            {
                Collect(other);
            }
        }

        public virtual void Collect(GameObject collector)
        {
            if (isCollected) return;
            isCollected = true;

            onCollectStarted?.Invoke(this);

            if (collectEffect != null)
            {
                collectEffect.Play();
            }

            if (collectSound != null)
            {
                collectSound.Play();
            }

            onCollect?.Invoke(this);

            if (destroyOnCollect)
            {
                if (destroyDelay > 0)
                {

                    foreach (var collider in _colliders)
                        collider.enabled = false;

                    StartCoroutine(DestroyAfterDelay());
                }
                else
                {
                    OnCollectFinished();
                    Destroy(gameObject);
                }
            }
            else
            {
                OnCollectFinished();
                gameObject.SetActive(false);
            }
        }

        private System.Collections.IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(destroyDelay);

            OnCollectFinished();
            Destroy(gameObject);
        }

        private void OnCollectFinished()
        {
            onCollectFinished?.Invoke(this);
        }

        public bool IsCollected()
        {
            return isCollected;
        }
    }
}