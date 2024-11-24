namespace OravaGames.Events
{
	using System.Collections;
	using UnityEngine;
	using UnityEngine.Events;

#if UNITY_EDITOR
	using UnityEditor;
#endif

	public class GameEventListener : MonoBehaviour
	{
		[Tooltip("Event to register with.")]
		public GameEvent Event;

		[Tooltip("Response to invoke when Event is raised.")]
		public UnityEvent Response;

		[Tooltip("Delay in seconds to invoke a Response when Event is raised.")]
		public float Delay = 0f;

		public bool CheckTriggered = false;

		private void OnEnable()
		{
			Event.RegisterListener(this);

			if (CheckTriggered && Event.Invoked)
				OnEventInvoked();
		}

		private void OnDisable()
		{
			StopAllCoroutines();
			Event.UnregisterListener(this);
		}

		public void OnEventInvoked()
		{
			if (Delay > 0.0f)
				StartCoroutine(DelayedResponse(Response, Delay));
			else
				Response?.Invoke();
		}

		private IEnumerator DelayedResponse(UnityEvent response, float delay)
		{
			yield return new WaitForSeconds(delay);
			response?.Invoke();
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(GameEventListener))]
	public class GameEventListenerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			GUI.enabled = Application.isPlaying;
			GameEventListener e = target as GameEventListener;
			if (GUILayout.Button("Invoke"))
				e.OnEventInvoked();
		}
	}
#endif
}
