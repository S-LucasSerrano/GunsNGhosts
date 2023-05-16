using UnityEngine;
using UnityEngine.Events;
using GunsNGhosts.Damage;

namespace GunsNGhosts.Ghosts
{
	public class GhostHealth : Health, IRequire<Ghost> 
	{
		/// <summary> Animator of this Ghost. </summary>
		Animator animator = null;

		/// <summary> Distance to translate backwards when damaged. </summary>
		[Space] [SerializeField] float knokback = .1f;

		/// <summary> AudioSource played when this Ghost gets damage. </summary>
		[Space] [SerializeField] AudioSource damageSound = null;
		/// <summary> Max difference from the original pithch to play the damage sound. </summary>
		[SerializeField] float pitchRandomization = 0.01f;
		/// <summary> Original pitch of the damage sound. </summary>
		float originalPitch = 0;

		/// <summary> Particles played when this Ghost dies. </summary>
		[SerializeField] ParticleSystem deathParticles = null;

		/// <summary> Event invoked when this Ghost dies. </summary>
		UnityEvent onDeath = new UnityEvent();


		// ---------------------------------------------------------------

		public void SetRequirement(Ghost requirement)
		{
			animator = requirement.Animator;

			originalPitch = damageSound.pitch;
			deathParticles.transform.parent = transform.parent;
		}


		// ---------------------------------------------------------------
		#region Health

		/// Function called when taking damage.
		protected override void Damaged(int damage, Component source = null)
		{
			Vector3 dir = transform.position - source.transform.position;
			transform.position += dir.normalized * knokback;

			animator.SetTrigger("Damage");

			damageSound.pitch = Random.Range(originalPitch - pitchRandomization, originalPitch  + pitchRandomization);
			damageSound.PlayOneShot(damageSound.clip);
		}

		/// Function called when the health reaches 0.
		protected override void Die(Component source = null)
		{ 
			// Play particles pointing to the away from the damage source.
			deathParticles.transform.position = transform.position;
			Vector3 dir = transform.position - source.transform.position;
			deathParticles.transform.rotation =	Quaternion.LookRotation( dir, Vector2.up );
			deathParticles.Clear();
			deathParticles.Play();

			// Reset this Ghost health.
			currentHealth = maxHealth;

			/* Habra que devolverlo a la pool. */

			Vector3 pos = new Vector3();
			pos.x = Random.Range(1f, -1f);
			pos.y = Random.Range(1f, -1f);
			pos.z = 0;
			transform.position = pos.normalized * 10;

			onDeath.Invoke();
		}

		#endregion


		// ---------------------------------------------------------------
		#region Properties

		/// <summary> Event invoked when this Ghost dies. </summary>
		public UnityEvent OnDeath => onDeath;

		#endregion
	}
}
