using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GunsNGhosts;

public class CameraShakeArea : MonoBehaviour
{
	// Cam shake added when the player is as far as it can be.
	[Space] [SerializeField] [Range(0, 1)] float minCamShake = 0;
	// Cam shake added where the player is as close as it can be.
	[SerializeField] [Range(0, 1)] float maxCamShake = 1;

	CameraShaker camShaker = null;

	private void Start()
	{
		camShaker = Game.Instance.GetReference<CameraShaker>();
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.transform == Game.Instance.GetReference<Player>().Transform)
		{
			Vector3 dis = transform.position - collision.transform.position;
			float value = dis.magnitude / transform.lossyScale.x;
			value = Mathf.Lerp( minCamShake, maxCamShake, value);

			camShaker.AddLimitedShake(value);
		}
	}
}
