namespace Secrets.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    public class PulsingGraphic : MonoBehaviour
    {
        [SerializeField] Graphic _graphic;

        [field: SerializeField] float Speed { get; set; } = 1f;
        [field: SerializeField] Color ColorA { get; set; } = Color.white;
        [field: SerializeField] Color ColorB { get; set; } = Color.gray;

        void Awake()
        {
            if (_graphic == null)
                _graphic = GetComponent<Graphic>();
        }

        void Update()
        {
            _graphic.color = Color.Lerp(ColorA, ColorB, Mathf.PingPong(Time.time * Speed, 1));
        }
    }
}