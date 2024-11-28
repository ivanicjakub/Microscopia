namespace Secrets.Gameplay
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    public class Trigger : MonoBehaviour
    {
        [SerializeField] string tagCheck;
        [SerializeField] List<string> tagCheckAdditional = new List<string>();
        [SerializeField] UnityEvent onTriggerEnter;
        [SerializeField] UnityEvent onTriggerExit;

        public void OnTriggerEnter(Collider other)
        {
            if (string.IsNullOrEmpty(tagCheck) && (tagCheckAdditional == null || tagCheckAdditional.Count == 0))
                return;

            if (other.CompareTag(tagCheck) || tagCheckAdditional.Contains(other.tag))
            {
                onTriggerEnter?.Invoke();
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (string.IsNullOrEmpty(tagCheck) && (tagCheckAdditional == null || tagCheckAdditional.Count == 0))
                return;

            if (other.CompareTag(tagCheck) || tagCheckAdditional.Contains(other.tag))
            {
                onTriggerExit?.Invoke();
            }
        }
    }
}