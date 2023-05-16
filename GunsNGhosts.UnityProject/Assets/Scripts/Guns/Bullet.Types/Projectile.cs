using System.Collections;
using UnityEngine;

namespace GunsNGhosts.Guns
{
	/// <summary> A bullet that can change speed over time, an collide with walls. </summary>
	public class Projectile : Bullet
	{
		/// <summary> Direction of this projectile. </summary>
		Vector3 direction = Vector3.right;

		/// <summary> Speed of this projectile when is shot. </summary>
		[Space][SerializeField] float initialSpeed = 10;
		/// <summary> Current speed of this projectile. </summary>
		float speed = 0;
		/// <summary> Speed modifier per second. </summary>
		[SerializeField] float speedOverTime = -1f;

		/// <summary> Time that this projectile waits to destroy itselef haver been shot. </summary>
		[SerializeField] float lifeTime = 1;
		/// <summary> Current time remaining to destroy this projectile. </summary>
		float lifeTimeCounter = 0;

		/// <summary> Currently active movement routine. </summary>
		Coroutine movementRoutine = null;
		/// <summary> Currently active life time routine. </summary>
		Coroutine lifeTimeRoutine = null;

		[Space][SerializeField] float speedReductionOnContact = .2f;
		[SerializeField] float lifeReductionOnContact = .2f;

		[Space][SerializeField] LayerMask destroyOnTouching = 0;

		/// <summary> Particles played when this Bullet is destroyed. </summary>
		[Space] [SerializeField] ParticleSystem particles = null;


		// ----------------------------------------------------------------------
		#region Start

		private void Start()
		{
			if (particles != null)
				particles.transform.parent = transform.parent;
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Bullet

		public override void Shoot()
		{
			gameObject.SetActive(true);

			// Start moving.
			if (movementRoutine != null) StopCoroutine(movementRoutine);
			movementRoutine = StartCoroutine(MovementRoutine());

			// Start the routine that destroys this projectile.
			if (lifeTimeRoutine != null) StopCoroutine(lifeTimeRoutine);
			lifeTimeRoutine = StartCoroutine(LifeTimeRoutine());
		}

		/// <summary> Coroutine that moves this projectile. </summary>
		IEnumerator MovementRoutine()
		{
			speed = initialSpeed;
			direction = transform.right;
			direction.z = 0;

			while (speed > 0)
			{
				transform.position += direction.normalized * speed * Time.deltaTime;
				speed += speedOverTime * Time.deltaTime;

				transform.right = direction;

				yield return new WaitForFixedUpdate();
			}
		}

		/// <summary> Routine that destroys this projectile after some time. </summary>
		IEnumerator LifeTimeRoutine()
		{
			lifeTimeCounter = lifeTime;

			while (lifeTimeCounter > 0)
			{
				lifeTimeCounter -= Time.deltaTime;
				yield return null;
			}

			Destroy();
		}

		public override void Destroy()
		{
			// Particles.
			if (particles != null)
			{
				particles.transform.SetPositionAndRotation(transform.position, transform.rotation);
				particles.Play();
			}

			gameObject.SetActive(false);
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Collision

		private void OnCollisionEnter2D(Collision2D collision)
		{
			// Refelect when touching something.
			ContactPoint2D contact = collision.contacts[0];
			direction = Vector3.Reflect(direction, contact.normal);

			// Reduce speed and life time.
			speed -= speed * speedReductionOnContact;
			lifeTimeCounter -= lifeTimeCounter * lifeReductionOnContact;

			if (destroyOnTouching.ContainsLayer(collision.gameObject.layer))
				Destroy();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (destroyOnTouching.ContainsLayer(collision.gameObject.layer))
				Destroy();
		}

		#endregion
	}
}
