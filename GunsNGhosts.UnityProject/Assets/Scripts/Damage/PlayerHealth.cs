using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GunsNGhosts.Damage
{
	public class PlayerHealth : Health, IRequire<Player>
	{
		/// <summary> Reference to the player that this component is part of. </summary>
		Player player = null;

		/// <summary> Time the player is inmune after getting damage. </summary>
		[Header("   Damage")] [SerializeField] float inmuneTime = 0.5f;
		/// <summary> Is the player inmune to damage? </summary>
		bool inmune = false;
		/// <summary> Curretly active inmunity routine. </summary>
		Coroutine inmunityRoutine = null;

		/// <summary> Reference to the camera shaker component. </summary>
		CameraShaker camShaker = null;
		/// <summary> Camera shake added when getting damage. </summary>
		[SerializeField] [Range(0, 1)] float damageCamShake = 0.1f;
		/// <summary> Time that the game complitly freezes after getting damage. </summary>
		[SerializeField] float impactEffectDuration = 0.05f;
		/// <summary> Partciles played when getting damage. </summary>
		[SerializeField] ParticleSystem damageParticles = null;
		/// <summary> AudioSource played when getting damage. </summary>
		[SerializeField] AudioSource damageSound = null;

		/// <summary> Event invoked when getting damaged, passing the remaining health as a parameter. </summary>
		UnityEvent<int> onDamaged = new UnityEvent<int>();

		/// <summary> Value that the TimeScale is set to when the player dies. </summary>
		[Header("   Death")] [SerializeField] float deathSlowmotionScale = 0.5f;
		/// <summary> Duration of the slow motion effect after the player dies. </summary>
		[SerializeField] float deathSlowmotionDuration = 1f;
		/// <summary> Particles played when the player dies. </summary>
		[SerializeField] ParticleSystem deathParticles = null;
		/// <summary> AudioSource played when the player dies. </summary>
		[SerializeField] AudioSource deathSound = null;
		/// <summary> Projetile shot when the player dies. </summary>
		[SerializeField] GunsNGhosts.Guns.Projectile deadPlayerProjectile = null;

		/// <summary> Event invoked whem the player dies. </summary>
		UnityEvent onDeath = new UnityEvent();
		[Space] [SerializeField] GameEvent onDeathGameEvent = null;


		// ---------------------------------------------------------------------
		#region Initialization

		public void SetRequirement(Player requirement)
		{
			player = requirement;
		}

		protected override void Start()
		{
			base.Start();

			camShaker = ReferenceProvider.GetReference<CameraShaker>();

			deadPlayerProjectile.transform.position = Vector3.up * 10000;
		}

		#endregion


		// ---------------------------------------------------------------------
		#region Damage

		public override void TakeDamage(int damage, Component source = null)
		{
			// Make the player inmune for a time after getting damage.
			if (inmune) return;
			if (inmunityRoutine != null) StopCoroutine(inmunityRoutine);
			inmunityRoutine = StartCoroutine(InmunityRoutine());

			// Default damage behaviour.
			base.TakeDamage(damage, source);

			// Add camera shake.
			camShaker.AddLimitedShake(damageCamShake);

			Slowmotion(0, impactEffectDuration);    // Freeze the game.
			player.Animator.SetTrigger("Damage");   // Animation.
			damageParticles.Play();                 // Particles.
			damageSound.Play();						// Sound.

			// Fire the damage event.
			onDamaged.Invoke(currentHealth);
		}

		public void Heal(int amount)
		{
			currentHealth += amount;
			if (currentHealth > maxHealth)
				currentHealth = maxHealth;
		}

		IEnumerator InmunityRoutine()
		{
			inmune = true;
			yield return new WaitForSeconds(inmuneTime);
			inmune = false;
		}

		#endregion


		// ---------------------------------------------------------------------
		#region Death

		protected override void Die(Component source = null)
		{
			// Place the dead player projectile going away from the source of damage.
			deadPlayerProjectile.transform.position = transform.position;
			deadPlayerProjectile.transform.parent = transform.parent;

			if (source != null)
			{
				Vector3 dir = source.transform.position - transform.position;
				dir = Vector3.Cross(dir, Vector3.forward);
				deadPlayerProjectile.transform.LookAt(transform.position + Vector3.forward, dir);
			}

			deadPlayerProjectile.Shoot();

			// Slow motion effect.
			Slowmotion(deathSlowmotionScale, deathSlowmotionDuration, deadPlayerProjectile);
			// Particles.
			deathParticles.transform.parent = transform.parent;
			deathParticles.transform.position = transform.position;
			deathParticles.Play();
			// Sound.
			deathSound.transform.parent = transform.parent;
			deathSound.Play();

			// Fire the dead event.
			onDeath.Invoke();
			onDeathGameEvent.Invoke();

			gameObject.SetActive(false);
		}

		#endregion


		// ---------------------------------------------------------------------
		#region Slowmotion

		Coroutine slowmotionRoutine = null;

		/// <summary> Start a corroutine that chages the time scale for a given amount of time. </summary>
		/// <param name="target"> The MonoBehaviour that will start the corroutine. </param>
		/// When the player dies, this GameObject is turned off and cannot start coroutines.
		/// We allow to pass a MonoBehaviour to start the coroutine to make sure that it can still play after the player dies.
		void Slowmotion(float timeScale, float duration, MonoBehaviour target = null)
		{
			if (target == null)
				target = this;

			if (!target.enabled || !target.gameObject.activeInHierarchy)
				return;

			timeScale = Mathf.Clamp01(timeScale);

			if (slowmotionRoutine != null) StopCoroutine(slowmotionRoutine);
			slowmotionRoutine = target.StartCoroutine(SlowmotionRoutine(timeScale, duration));
		}

		IEnumerator SlowmotionRoutine(float timeScale, float duration)
		{
			Time.timeScale = timeScale;
			yield return new WaitForSecondsRealtime(duration);
			Time.timeScale = 1;
		}

		#endregion


		// ---------------------------------------------------------------------
		#region Properties

		/// <summary> Event invoked when getting damaged, passing the remaining health as a parameter. </summary>
		public UnityEvent<int> OnDamaged => onDamaged;

		/// <summary> Event invoked whem the player dies. </summary>
		public UnityEvent OnDeath => onDeath;

		#endregion
	}
}
