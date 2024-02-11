using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
	[Space] [SerializeField] LayerMask raycastMask = ~0;

	[SerializeField] float maxDistance = 5;

	Vector3 end = Vector3.zero;


	void OnEnable()
	{
		end = (transform.position + transform.forward * maxDistance);
	}

	private void Update()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, maxDistance, raycastMask);

		float size = hit ? hit.distance : maxDistance;
		transform.localScale = new Vector3(size, 1, 1);

		end = hit ? hit.point : (transform.position + transform.forward * maxDistance);
	}

	/// <summary> Position where the laser ends. </summary>
	public Vector3 End
	{
		get
		{
			if (this.isActiveAndEnabled)
				return end;
			else
				return transform.position;
		}
	}
}
