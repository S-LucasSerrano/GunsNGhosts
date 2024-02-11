using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
	[Space][SerializeField] GameEvent gameEvent = null;
	[SerializeField] UnityEvent onEventTrigger = new UnityEvent();


	private void Start()
	{
		gameEvent.AddListener(EventListener);
	}

	private void OnDestroy()
	{
		gameEvent.RemoveListener(EventListener);
	}

	void EventListener()
	{
		onEventTrigger.Invoke();
	}
}
