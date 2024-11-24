using UnityEngine;
using UnityEngine.Events;
namespace Secrets.Gameplay
{
    public class AnyKeyEvent : MonoBehaviour
    {
        [SerializeField] UnityEvent onAnyKeyDown;

        void Update()
        {
            if (Input.anyKeyDown)
                onAnyKeyDown?.Invoke();
        }
    }
}