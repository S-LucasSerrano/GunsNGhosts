using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component activates other when the object touches something in the specified layer. </summary>
public class MovingObjectDamager : MonoBehaviour
{
    [Space] [SerializeField] LayerMask wallLayers = new ();
    [SerializeField] GameObject damagerComponent = null;
	[SerializeField] GameObject self = null;

    List<GameObject> touchingWalls = new();


	private void Start()
	{
		damagerComponent.SetActive(false);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == self)
			return;

		if (wallLayers.ContainsLayer(other.gameObject.layer))
		{
			touchingWalls.Add(other.gameObject);

			damagerComponent.SetActive(true);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (touchingWalls.Contains(other.gameObject))
		{
			touchingWalls.Remove(other.gameObject);

			if (touchingWalls.Count == 0)
			{
				damagerComponent.SetActive(false);
			}
		}
	}
}
