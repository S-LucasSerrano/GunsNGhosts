using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LucasSerrano.Pooling;
using GunsNGhosts.Damage;

namespace GunsNGhosts.Guns
{
	public class BubbleGun : Gun
	{
		/// <summary> Projectile this gun shoots. </summary>
		[Space] [SerializeField] Projectile projectile = null;
		/// <summary> Number of bullets in the pool. </summary>
		[SerializeField] [Min(0)] int poolSize = 10;
		/// <summary> Pool to extract the bullets to shoot. </summary>
		FixedPool<Projectile> bulletPool;

		/// <summary> Time that takes to fully charge the bubble. </summary>
		[Space] [SerializeField] float maxChargingTime = 1f;
		float chargeTimer = 0;

		/// Definitions of how the projectile will be shot at min and max charge.
		[Space] [SerializeField] BubbleData bubbleDataAtCero = new();
		[SerializeField] BubbleData bubbleDataAtOne = new();
		/// Curve that defines how that charge that data goes from min to max.
		[Space][SerializeField] AnimationCurve bubbleDataCurve = AnimationCurve.Linear(0,0,1,1);

		[Space] [SerializeField] float sizeRandomization = 0.1f;


		/// <summary> Animator of the player character. </summary>
		Animator playerAnimator = null;

		/// <summary> Particles played at the shooting point when a  bullet is shot. </summary>
		[Space] [SerializeField] ParticleSystem shootingParticles = null;
		/// <summary> Particles played while charging. </summary>
		[SerializeField] ParticleSystem charginParticles = null;

		/// <summary> AudioSource that plays when shooting. </summary>
		[Space][SerializeField] AudioSource shootingAudio = null;
		[SerializeField] AudioSource chargingAudio;
		/// <summary> AudioSource that plays when shooting. </summary>
		[SerializeField] AudioSource emptyAudio = null;
		/// <summary> Max randomization from the original audio pitch. </summary>
		[SerializeField] float emptyPitchRandomization = 0.1f;
		/// <summary> Original pitch of the shooting audio. </summary>
		float emptyOriginalPitch = 1;


		// -----------------------------------------------------------
		#region Initialization

		private void Start()
		{
			// Initialize the bullet pool.
			bulletPool = new FixedPool<Projectile>(CreateBullet, poolSize);
			bulletPool.Initialize();

			// Find references to relevant components.
			playerAnimator = ReferenceProvider.GetReference<Player>().Animator;

			// Save original audio pitch.
			if (emptyAudio != null)
				emptyOriginalPitch = emptyAudio.pitch;

		}

		/// <summary> Instantiate a new bullet. </summary>
		Projectile CreateBullet()
		{
			Projectile newBullet = Instantiate(projectile, transform);
			newBullet.Destroy();
			return newBullet;
		}

		#endregion


		// -----------------------------------------------------------
		#region Gun Methods

		// When the player pressed the button, we restart the time counter.
		protected override void OnShootingStarts()
		{
			chargeTimer = 0;

			// If there is no ammo left, dont shoot.
			if (Shooter.Ammo <= 0)
			{
				PlayShootingAnimation();
				PlaySound(emptyAudio, emptyOriginalPitch, emptyPitchRandomization);
				return;
			}
			
			charginParticles.Play();
			chargingAudio.Play();
		}

		// While the player keeps the button pressed, we count the time.
		protected override void OnShootingMaintained()
		{
			if (chargeTimer >= maxChargingTime)
				return;

			chargeTimer += Time.deltaTime;

			// Keep the particles in position.
			charginParticles.transform.SetPositionAndRotation(ShootingPoint.position, ShootingPoint.rotation);

			// If the counter reaches the max, we release the bubble.
			if (chargeTimer >= maxChargingTime)
				ReleaseBubble(1);
		}

		// When the player releases, we release a bubble.
		protected override void OnShootingEnded()
		{
			// Stop the charging effects.
			charginParticles.Stop();
			chargingAudio.Stop();

			// Only if the bubble has not been released yet by reaching the max time.
			if (chargeTimer >= maxChargingTime)
				return;

			ReleaseBubble(chargeTimer / maxChargingTime);
		}

		public override void Unequip()
		{
			// Stop the charging effects.
			charginParticles.Stop();
			chargingAudio.Stop();
		}

		#endregion

		// -----------------------------
		#region Shoot a Bubble

		/// <summary> Place a bullet based on a charge value between 0 and 1. </summary>
		void ReleaseBubble(float charge)
		{
			// If there is no ammo left, dont shoot.
			if (Shooter.Ammo <= 0)
			{
				PlayShootingAnimation();
				PlaySound(emptyAudio, emptyOriginalPitch, emptyPitchRandomization);
				return;
			}

			// Get the next bullet from the pool.
			Projectile projectile = bulletPool.Get();
			projectile.transform.SetPositionAndRotation(ShootingPoint.position, ShootingPoint.rotation);

			// Get a value between 0 and 1 that tell us how much the player charged the gun.
			charge = Mathf.Clamp01(charge);
			charge = bubbleDataCurve.Evaluate(charge);

			// Use ammo based on charge
			float ammo = Mathf.Lerp(bubbleDataAtCero.ammo, bubbleDataAtOne.ammo, charge);
			Shooter.UseAmmo(Mathf.RoundToInt(ammo));

			// Give the projectile values based on the charge.
			projectile.initialSpeed = Mathf.Lerp(bubbleDataAtCero.initialSpeed, bubbleDataAtOne.initialSpeed, charge );
			projectile.speedOverTime = Mathf.Lerp(bubbleDataAtCero.speedOverTime, bubbleDataAtOne.speedOverTime, charge );
			projectile.lifeTime = Mathf.Lerp(bubbleDataAtCero.lifeTime, bubbleDataAtOne.lifeTime, charge );

			float damage = Mathf.Lerp(bubbleDataAtCero.damage, bubbleDataAtOne.damage, charge);
			projectile.GetComponent<ConstantDamager>().damage = Mathf.RoundToInt(damage);

			projectile.transform.localScale = Vector3.one *
				(Mathf.Lerp(bubbleDataAtCero.size, bubbleDataAtOne.size, charge )
				+ (Random.Range(-sizeRandomization, sizeRandomization)));

			projectile.Shoot();

			// Play an audio pitch based on the charge.
			shootingAudio.pitch = Mathf.Lerp( bubbleDataAtCero.audioPitch, bubbleDataAtOne.audioPitch, charge );
			shootingAudio.PlayOneShot( shootingAudio.clip );

			PlayShootingAnimation();        // Animation.
			PlayShootingParticles();        // Particles.

			// Stop the charging effects.
			charginParticles.Stop();
			chargingAudio.Stop();
		}

		#endregion

		// -----------------------------
		#region Shooting FX

		private void PlayShootingAnimation()
		{
			if (playerAnimator == null) return;
			playerAnimator.SetTrigger("Shoot");
		}

		private void PlayShootingParticles()
		{
			if (shootingParticles == null) return;
			shootingParticles.transform.SetPositionAndRotation(ShootingPoint.position, ShootingPoint.rotation);
			shootingParticles.Play();
		}

		private void PlaySound(AudioSource audioSource, float originalPitch, float pitchRandomization)
		{
			if (audioSource == null)
				return;

			float randomPitch = originalPitch + Random.Range(-pitchRandomization, pitchRandomization);

			audioSource.pitch = randomPitch;
			audioSource.PlayOneShot(audioSource.clip);
		}

		#endregion


		// -----------------------------------------------------------
		#region Definitions

		/// <summary> That that defines how the projectile will be shot. </summary>
		[System.Serializable]
		public struct BubbleData
		{
			public float size;

			[Space]
			public int damage;
			public int ammo;

			[Space]
			public float initialSpeed;
			public float speedOverTime;
			public float lifeTime;

			[Space]
			public float audioPitch;
		}

		#endregion
	}
}
