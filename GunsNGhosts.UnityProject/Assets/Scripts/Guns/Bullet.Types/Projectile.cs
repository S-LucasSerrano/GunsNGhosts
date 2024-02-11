using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GunsNGhosts.Guns
{
	/// <summary> A bullet that can change speed over time, an collide with walls. </summary>
	public class Projectile : Bullet
	{
		/// <summary> Direction of this projectile. </summary>
		Vector3 direction = Vector3.right;

		/// <summary> Speed of this projectile when is shot. </summary>
		[Space]public float initialSpeed = 10;
		/// <summary> Current speed of this projectile. </summary>
		float speed = 0;
		/// <summary> Speed modifier per second. </summary>
		public float speedOverTime = -1f;

		/// <summary> Time that this projectile waits to destroy itselef haver been shot. </summary>
		public float lifeTime = 1;
		/// <summary> Current time remaining to destroy this projectile. </summary>
		float lifeTimeCounter = 0;

		/// <summary> If true, this projectile always look in the direction its moving with its right vector. </summary>
		[Space][SerializeField] bool lookInMovementDirection = true;
		/// <summary> Currently active movement routine. </summary>
		Coroutine movementRoutine = null;
		/// <summary> Currently active life time routine. </summary>
		Coroutine lifeTimeRoutine = null;

		[Space][SerializeField] float speedReductionOnContact = .2f;
		[SerializeField] float lifeReductionOnContact = .2f;

		[Space] [SerializeField] LayerMask collideWith = -1;
		[SerializeField] LayerMask destroyOnTouching = 0;

		/// <summary> Particles played when this Bullet is destroyed. </summary>
		[Space] [SerializeField] ParticleSystem particles = null;

		/// <summary> Event invoked when this projectile is destroyed. </summary>
		[HideInInspector] public UnityEvent onDestroyed = new();


		// ----------------------------------------------------------------------
		#region Bullet

		public override void Shoot()
		{
			Shoot(transform.right);
		}

		public void Shoot(Vector3 dir)
		{
			gameObject.SetActive(true);
			direction = dir;

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
			direction.z = 0;

			while (speed > 0)
			{
				transform.position += direction.normalized * speed * Time.deltaTime;
				speed += speedOverTime * Time.deltaTime;

				if (lookInMovementDirection)
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
				particles.transform.parent = transform.parent;
				particles.transform.SetPositionAndRotation(transform.position, transform.rotation);
				particles.Play();
			}

			gameObject.SetActive(false);

			onDestroyed.Invoke();
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Collision

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collideWith.ContainsLayer(collision.gameObject.layer) == false)
				return;

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
