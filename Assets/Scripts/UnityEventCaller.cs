namespace Secrets.Gameplay
{
#if UNITY_EDITOR
	using UnityEditor;
#endif
	using UnityEngine;
	using UnityEngine.Events;
	public class UnityEventCaller : MonoBehaviour
	{
		public UnityEvent mainEvent;
		public UnityEvent resetEvent;
		public bool isOn = false;

		public void InvokeMain()
		{
			if (mainEvent != null)
				mainEvent.Invoke();
			isOn = true;
		}

		public void InvokeReset()
		{
			if (resetEvent != null)
				resetEvent.Invoke();
			isOn = false;
		}

		public void SetState(bool on)
		{
			if (on)
				InvokeMain();
			else
				InvokeReset();
		}

		public void TrySetState(bool on)
		{
			if (isOn != on)
				SetState(on);
		}

		public void ToggleState()
		{
			SetState(!isOn);
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(UnityEventCaller))]
	public class UnityEventCallerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUI.enabled = Application.isPlaying;

			UnityEventCaller e = target as UnityEventCaller;
			if (GUILayout.Button("Invoke Main"))
				e.InvokeMain();
			if (GUILayout.Button("Invoke Reset"))
				e.InvokeReset();
		}
	}
#endif
}
