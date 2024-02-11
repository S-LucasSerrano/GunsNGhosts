using UnityEngine;

public class DiscMovement : MonoBehaviour
{
    [Space] [SerializeField] float speed = 10;
    Vector3 direction = new();

    [SerializeField] LayerMask collisionLayer = new();

    [Space] [SerializeField] ParticleSystem collisionParticles = null;


    // ----------------------------------------------------------

    void Start()
    {
        direction.x = Random.Range(-1f,1f);
        direction.y = Random.Range(-1f,1f);
    }

	private void FixedUpdate()
	{
        transform.position += direction.normalized * Time.deltaTime * speed;
	}


	// ----------------------------------------------------------

	private void OnCollisionEnter2D(Collision2D collision)
	{
        if (!collisionLayer.ContainsLayer(collision.gameObject.layer))
            return;

        collisionParticles.transform.position = collision.contacts[0].point;
        collisionParticles.Play();

        direction = Vector3.Reflect( direction, collision.contacts[0].normal );
        direction.z = 0;
	}
}
