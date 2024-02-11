using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

/// <summary>
/// ScriptableObject that has a reference to a UnityEvent. <br/>
/// Allowing to decouple the invoker and the listener. </summary>
[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
	[Space] [SerializeField] UnityEvent unityEvent = new();


	public void Invoke()
	{
		unityEvent.Invoke();
	}

	public void AddListener(UnityAction call)
	{
		unityEvent.AddListener(call);
	}

	public void RemoveListener(UnityAction call)
	{
		unityEvent.RemoveListener(call);
	}
}


// =================================================================
#if UNITY_EDITOR

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if(GUILayout.Button("Invoke"))
		{
			if (!Application.isPlaying)
			{
				Debug.Log("GameEvents can only be invoked in play mode.");
				return;
			}

			((GameEvent)target).Invoke();
		}
	}
}

#endif
