using UnityEngine;

namespace GunsNGhosts.Guns
{
	/// <summary> Simple Bullet that moves forward, damages Ghosts and is destroyed when touches something. </summary>
	public class SimpleBullet : Bullet
	{
		/// <summary> Speed of this Bullet. </summary>
		[Space] [SerializeField] float speed = 10;
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

		private void FixedUpdate()
		{
			transform.position += transform.right * speed * Time.deltaTime;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			Destroy();
		}


		// ----------------------------------------------------------------------
		#region Bullet

		public override void Shoot()
		{
			gameObject.SetActive(true);
		}

		public override void Destroy()
		{
			// Particles.
			if (particles != null)
			{
				particles.transform.SetPositionAndRotation(transform.position, transform.rotation);
				particles.Play();
			}

			// "Destroy" this Bullet.
			gameObject.SetActive(false);
		}

		#endregion
	}
}
