using UnityEngine;

public class SizeRandomizer : MonoBehaviour
{
    [SerializeField] float scaleRandomization = 0.1f;
	Vector3 originalScale = Vector3.zero;

	bool started = false;


	private void Start()
	{
		started = true;

		originalScale = transform.localScale;
		OnEnable();
	}


	private void OnEnable()
	{
		if (!started) { return; }

		float randomScaler = 1 + (Random.Range(-scaleRandomization, scaleRandomization));
		transform.localScale =  originalScale * randomScaler;
	}
}
