using System.Collections;
using UnityEngine;
using LucasSerrano.Pooling;

namespace GunsNGhosts.Guns
{
	/// <summary> Simple Gun that shoots bullets. </summary>
	public class SimpleGun : Gun
	{
		/// <summary> Prefab of the bullet that will be instantiated. </summary>
		[Space] [SerializeField] Bullet bulletPrefab = null;
		/// <summary> Number of bullets in the pool. </summary>
		[SerializeField] [Min(0)] int poolSize = 10;
		/// <summary> Pool to extract the bullets to shoot. </summary>
		FixedPool<Bullet> bulletPool;

		/// <summary> Does this Gun keep shotting without the need to release the button? </summary>
		[Space] [SerializeField] bool automatic = true;

		/// <summary> Amount of bullets that are shot. </summary>
		[Space][SerializeField] int bulletsPerShot = 1;
		/// <summary> Max distance from the ShootingPoint that a bullet can be shot at. </summary>
		[SerializeField] float randomizationPosition = 0;
		/// <summary> Max angle that a bullet can be shot at. </summary>
		[SerializeField] float randomizationAngle = 0;

		/// <summary> Min time between shots. </summary>
		[SerializeField] float shootingCooldown = 0.5f;
		/// <summary> Is this gun is waiting the cooldown after shooting? </summary>
		bool coolingdown = false;
		/// <summary> Currently active cooldown routine. </summary>
		Coroutine cooldownRoutine = null;

		/// <summary> Distance the player moves backwards when shooting. </summary>
		[Space] [SerializeField] float recoil = 0;
		/// <summary> Reference to player transform. </summary>
		Transform playerTransform = null;

		/// <summary> Animator of the player character. </summary>
		Animator playerAnimator = null;

		/// <summary> Reference to the Game's camera shaker. </summary>
		CameraShaker cameraShaker = null;
		/// <summary> Camera shake added whem shooting. </summary>
		[Space] [SerializeField] [Range(0,1)] float camShake = 0.3f;
		/// <summary> Time that takes the camera to return to its position after the directional shake. </summary>
		[SerializeField] float camShakeRecoveryTime = .1f;

		/// <summary> Particles played at the shooting point when a  bullet is shot. </summary>
		[Space][SerializeField] ParticleSystem shootingParticles = null;

		/// <summary> AudioSource that plays when shooting. </summary>
		[Space] [SerializeField] AudioSource shootingAudio = null;
		/// <summary> Max randomization for the shooting audio pitch. </summary>
		[SerializeField] float shootingPitchRandomization = 0.1f;
		/// <summary> Original pitch of the shooting audio. </summary>
		float shootingOriginalPitch = 1;
		/// <summary> AudioSource that plays when shooting whit no bullets. </summary>
		[SerializeField] AudioSource emptyAudio = null;
		/// <summary> Max randomization for the empty audio pitch. </summary>
		[SerializeField] float emptyPitchRandomization = 0.1f;
		/// <summary> Original pitch of the empty audio. </summary>
		float emptyOriginalPitch = 1;


		// ----------------------------------------------------------------------
		#region Start

		private void Start()
		{
			// Find references to relevant components.
			cameraShaker = ReferenceProvider.GetReference<CameraShaker>();
			playerTransform = ReferenceProvider.GetReference<Player>().Transform;
			playerAnimator = ReferenceProvider.GetReference<Player>().Animator;

			// Save original audio pitch.
			if (shootingAudio != null)
				shootingOriginalPitch = shootingAudio.pitch;
			if (emptyAudio != null)
				emptyOriginalPitch = emptyAudio.pitch;

			// Initialize the bullet pool.
			bulletPool = new FixedPool<Bullet>(CreateBullet, poolSize);
			bulletPool.Initialize();

			bulletPrefab.gameObject.SetActive(false);
		}

		/// <summary> Instantiate a new bullet. </summary>
		Bullet CreateBullet()
		{
			Bullet newBullet = Instantiate( bulletPrefab, transform );
			newBullet.Destroy();
			return newBullet;
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Gun

		/// Shoot when pressing the button if this is not an automatic Gun.
		protected override void OnShootingStarts()
		{
			if (automatic == true || coolingdown) return;

			Shoot();
		}

		/// Shoot while mainting the button if this is an automatic Gun.
		protected override void OnShootingMaintained()
		{
			if (automatic == false || coolingdown) return;

			Shoot();
		}

		#endregion


		// --------------------------------
		#region Shoot

		public virtual void Shoot()
		{
			// Wait the cooldown time to be able to shoot again.
			if (cooldownRoutine != null) StopCoroutine(cooldownRoutine);
			cooldownRoutine = StartCoroutine(CooldownRoutine());

			// If there is no ammo left, dont shoot.
			if (Shooter.Ammo <= 0)
			{
				PlayShootingAnimation();
				PlaySound(emptyAudio, emptyOriginalPitch, emptyPitchRandomization);
				return;
			}

			// Place the bullets in the shooting point.
			for (int i = 0; i < bulletsPerShot; i++)
			{
				// Get the next bullet from the pool.
				Bullet bullet = bulletPool.Get();
				bullet.transform.SetPositionAndRotation( ShootingPoint.position, ShootingPoint.rotation );

				// Randomize the starting position.
				float randomValue = Random.Range( -randomizationPosition, randomizationPosition );
				bullet.transform.position += ShootingPoint.up * randomValue;

				// Randomize the starting rotation.
				randomValue = Random.Range(-randomizationAngle, randomizationAngle);
				bullet.transform.Rotate( Vector3.forward * randomValue );

				bullet.Shoot();

				// If there is no more ammo, dont place more bullets.
				if (!Shooter.UseAmmo(Ammo))
					break;
			}

			// Recoil.
			Vector3 recoilDir = -ShootingPoint.right;
			recoilDir.z = 0;
			playerTransform.Translate( recoilDir.normalized * recoil );

			PlayShootingAnimation();        // Animation.
			PlayCameraShake();              // Camera Shake.
			PlayShootingParticles();        // Particles.
											// Audio.
			PlaySound( shootingAudio, shootingOriginalPitch, shootingPitchRandomization );			
		}

		/// <summary> Wait the cooldown time to be able to shoot again. </summary>
		protected IEnumerator CooldownRoutine()
		{
			coolingdown = true;
			yield return new WaitForSeconds(shootingCooldown);
			coolingdown = false;
		}

		#endregion

		#region Shooting FX

		private void PlayShootingAnimation()
		{
			if (playerAnimator == null) return;
			playerAnimator.SetTrigger("Shoot");
		}

		private void PlayCameraShake()
		{
			if (cameraShaker == null) return;
			cameraShaker.DirectionalShake(-ShootingPoint.right * camShake, camShakeRecoveryTime);
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
			//audioSource.Play();
		}

		#endregion
	}
}
