namespace Secrets.Gameplay
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    public class InputEvent : MonoBehaviour
    {
        [System.Serializable]
        public class KeyCodeEvent
        {
            public KeyCode[] keyCodes;
            public UnityEvent onKeysPressed;

            public bool IsAnyKeyPressed()
            {
                foreach (KeyCode key in keyCodes)
                {
                    if (Input.GetKeyDown(key))
                        return true;
                }
                return false;
            }

            public bool Containts(KeyCode keyCheck)
            {
                foreach (KeyCode key in keyCodes)
                {
                    if (key == keyCheck)
                        return true;
                }
                return false;
            }
        }

        public List<KeyCodeEvent> keyEvents = new List<KeyCodeEvent>();

        private void Update()
        {
            foreach (KeyCodeEvent keyEvent in keyEvents)
            {
                if(keyEvent.IsAnyKeyPressed())
                    keyEvent.onKeysPressed.Invoke();
            }
        }

        public void InvokeEvents()
        {
            foreach (KeyCodeEvent keyEvent in keyEvents)
            {
                keyEvent.onKeysPressed.Invoke();
            }
        }
    }
}