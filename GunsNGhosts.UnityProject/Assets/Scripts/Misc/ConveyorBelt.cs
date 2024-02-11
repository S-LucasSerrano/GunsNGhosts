using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Space] [SerializeField] LayerMask layerMask = new();
    [SerializeField] float speed = 1;


	private void OnTriggerStay2D(Collider2D collision)
	{
		if (layerMask.ContainsLayer(collision.gameObject.layer))
		{
			collision.transform.position += transform.right * speed * Time.deltaTime;
		}
	}
}
