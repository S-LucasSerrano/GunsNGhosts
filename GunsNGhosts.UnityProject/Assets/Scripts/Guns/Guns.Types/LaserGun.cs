using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunsNGhosts.Guns
{
    public class LaserGun : Gun
    {
        [Space] [SerializeField] GameObject laserPivot = null;
		[SerializeField] Laser laser = null;

		[SerializeField] float useAmmoEvery = .25f;

		/// <summary> Distance the player moves backwards when shooting. </summary>
		[Space] [SerializeField] float recoil = 0;
		/// <summary> Reference to player transform. </summary>
		Transform playerTransform = null;
		/// <summary> Animator of the player character. </summary>
		Animator playerAnimator = null;

		[Space][SerializeField] AudioSource laserSound = null;
		[SerializeField] AudioSource emptySound = null;

		[Space] [SerializeField] ParticleSystem endParticles = null;

		/// <summary> Reference to the Game's camera shaker. </summary>
		CameraShaker cameraShaker = null;
		/// <summary> Camera shake added while shooting. </summary>
		[Space][SerializeField] [Range(0, 1)] float camShake = 0.1f;


		float timeCounter = 0;


		// ------------------------------------------------------------

		private void Start()
		{
			// Find references to relevant components.
			playerTransform = ReferenceProvider.GetReference<Player>().Transform;
			playerAnimator = ReferenceProvider.GetReference<Player>().Animator;
			cameraShaker = ReferenceProvider.GetReference<CameraShaker>();

			laserPivot.SetActive(false);
		}


		// ------------------------------------------------------------

		protected override void OnShootingStarts()
		{
			// Play shooting animation.
			if (playerAnimator != null) playerAnimator.SetTrigger("Shoot");

			if (Shooter.Ammo <= 0)
			{
				// Play empty sound.
				emptySound.Play();

				return;
			}
			Shooter.UseAmmo(Ammo);

			// Activate the laser.
			laserPivot.SetActive(true);
			laserPivot.transform.parent = ShootingPoint;
			laserPivot.transform.position = ShootingPoint.position;
			laserPivot.transform.rotation = ShootingPoint.rotation;
			laserSound.Play();
			// Reset time counter.
			timeCounter = 0;

			// Start particles.
			endParticles.Play();
			// Recoil.
			if (playerTransform != null) playerTransform.Translate(-ShootingPoint.right * recoil);

		}

		protected override void OnShootingMaintained()
		{
			timeCounter += Time.deltaTime;
			if (timeCounter >= useAmmoEvery)		// Every x seconds...
			{
				// Reset time counter.
				Shooter.UseAmmo(Ammo);
				timeCounter = 0;

				// Play shooting animation.
				if (playerAnimator != null) playerAnimator.SetTrigger("Shoot");

				if (Shooter.Ammo > 0)
				{
					// Recoil.
					if (playerTransform != null) playerTransform.Translate(-ShootingPoint.right * recoil);
				}
				// If there is no ammo left:
				else
				{ 
					laserSound.Stop();
					laserPivot.SetActive(false);
					emptySound.Play();
					endParticles.Stop();
				}
			}

			if (Shooter.Ammo > 0)
			{
				// Cam shake.
				if (cameraShaker != null) cameraShaker.AddLimitedShake(camShake);
				// Place particles at the end of the laser.
				endParticles.transform.position = laser.End;
			}
		}

		protected override void OnShootingEnded()
		{
			laserPivot.SetActive(false);
			laserSound.Pause();

			endParticles.transform.position = Vector3.up * 1000;
			endParticles.Clear();
			endParticles.Stop();
		}

		public override void Unequip()
		{
			OnShootingEnded();
		}
	}
}
