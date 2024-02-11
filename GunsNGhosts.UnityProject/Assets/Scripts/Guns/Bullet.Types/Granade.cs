using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunsNGhosts.Guns
{
	[RequireComponent(typeof(Projectile))]
	public class Granade : MonoBehaviour
	{
		Projectile projectile = null;

		[Space] [SerializeField] GameObject explosion = null;
		[SerializeField] Animator explosionAnimator = null;
		[SerializeField] ParticleSystem explosionParticles = null;

		[Space] [SerializeField] AudioSource explosionAudio = null;

		[Space][SerializeField] ShakeByDistanceData camShake = new();
		CameraShaker camShaker = null;
		Transform playerTransform = null;



		// ----------------------------------------------------------

		private void Start()
		{
			projectile = GetComponent<Projectile>();
			projectile.onDestroyed.AddListener(OnProjectileDestroyed);

			explosion.transform.parent = transform.parent;
			explosion.SetActive(false);

			camShaker = Game.Instance.GetReference<CameraShaker>();
			playerTransform = Game.Instance.GetReference<Player>().Transform;
		}

		void OnProjectileDestroyed()
		{
			explosion.transform.position = transform.position;
			explosion.SetActive(true);
			explosionAnimator.SetTrigger("explode");
			explosionParticles.Play();

			explosionAudio.Play();

			camShaker.AddShakeByDistance(playerTransform.position, camShake);
		}
	}
}
