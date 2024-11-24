namespace OravaGames.Events
{
#if UNITY_EDITOR
	using UnityEditor;
#endif
	using System.Collections.Generic;
	using UnityEngine;
	using System;

	[CreateAssetMenu(menuName = "Orava Games/Game Event")]
	public class GameEvent : ScriptableObject
	{
		/// <summary>
		/// The list of listeners that this event will notify if it is raised.
		/// </summary>
		[NonSerialized]
		internal List<GameEventListener> eventListeners = new();

		[NonSerialized]
		internal bool _invoked;

		public bool Invoked { get => _invoked; }

		public void Invoke()
		{
			InvokeLocal();
		}

		private void InvokeLocal()
		{
			Debug.Log("Event " + name + " invoked.");
			for (int i = eventListeners.Count - 1; i >= 0; i--)
				eventListeners[i].OnEventInvoked();

			_invoked = true;
		}

		public void RegisterListener(GameEventListener listener)
		{
			if (!eventListeners.Contains(listener))
				eventListeners.Add(listener);
		}

		public void UnregisterListener(GameEventListener listener)
		{
			if (eventListeners.Contains(listener))
				eventListeners.Remove(listener);
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(GameEvent))]
	public class GameEventEditor : Editor
	{
		string ListenerLabel(GameEventListener listener)
		{
			var result = "Delay " + listener.Delay + "s ";
			return result;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			GameEvent e = target as GameEvent;

			GUI.enabled = false;
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("Registered listeners " + e.eventListeners.Count, EditorStyles.boldLabel);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			for (int i = 0; i < e.eventListeners.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.ObjectField(e.eventListeners[i], typeof(GameObject), true);
				EditorGUILayout.LabelField(ListenerLabel(e.eventListeners[i]));
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();
			GUI.enabled = Application.isPlaying;
			if (GUILayout.Button("Invoke"))
				e.Invoke();
		}
	}
#endif
}
