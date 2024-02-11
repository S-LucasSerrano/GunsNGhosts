using UnityEngine;
using UnityEngine.Events;
using TMPro;
using GunsNGhosts.Damage;

namespace GunsNGhosts.Guns
{
	/// <summary> Health component that changes the player's Gun when the health reaches 0 or the player touches it. </summary>
    public class GunPackage : Health, ISpawneable
    {
		GunSpawnManager manager = null;
		/// <summary> The gun that this package equips to the player. </summary>
		Gun gun = null;
		/// <summary> The player component that shoots the Gun. </summary>
		GunsNGhosts.CharacterController.GunShooter shooter = null;
		Transform playerTransform = null;

		/// <summary> UI text where the Gun name is written. </summary>
		[Space] [SerializeField] TextMeshProUGUI gunNameText = null;
		/// <summary> UI text where the Gun initial letter is written. </summary>
		[SerializeField] TextMeshProUGUI gunLetterText = null;

		/// <summary> Distance to translate backwards when damaged. </summary>
		[Space] [SerializeField] float knokback = .1f;

		/// <summary> Animator of this package. </summary>
		[Space] [SerializeField] Animator animator = null;

		/// Sound.
		[Space] [SerializeField] AudioSource damageSound = null;
		[SerializeField] AudioSource destroySound = null;
		/// Camera Shake.
		CameraShaker camShaker = null;
		[Space][SerializeField][Range(0,1)] float camShake = 0.3f;
		/// Particles.
		[Space][SerializeField] ParticleSystem destroyParticles = null;

		/// <summary> Event fired when destroying this package. </summary>
		UnityEvent onDestroy = new UnityEvent();


		// -----------------------------------------------------------------

		private void Reset()
		{
			animator = GetComponent<Animator>();
		}

		protected override void Start()
		{
			base.Start();

			shooter = ReferenceProvider.GetReference<Player>().Shooter;
			playerTransform = ReferenceProvider.GetReference<Player>().Transform;
			camShaker = ReferenceProvider.GetReference<CameraShaker>();
		}

		public void Respawn()
		{
			if (manager == null)
				manager = FindObjectOfType<GunSpawnManager>();

			gun = manager.GetRandomGun();
			gunNameText.text = gun.DisplayName;
			gunLetterText.text = gun.DisplayLetter;
		}


		// -----------------------------------------------------------------

		protected override void Damaged(int damage, Component source = null)
		{
			// Knockbak.
			transform.position += (transform.position - source.transform.position).normalized * knokback;
			// Animation.
			animator.SetTrigger("Damage");
			// Sound.
			damageSound.Play();
		}

		protected override void Die(Component source = null)
		{
			// Equip the player Gun.
			shooter.Equip(gun);

			// Sound.
			destroySound.transform.parent = transform.parent;
			destroySound.Play();
			// Particles.
			destroyParticles.transform.parent = transform.parent;
			destroyParticles.transform.position = transform.position;
			destroyParticles.Play();
			// CamShake.
			camShaker.AddLimitedShake( camShake );

			onDestroy.Invoke();
			gameObject.SetActive(false);
		}


		// -----------------------------------------------------------------

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.transform == playerTransform)
				Die();
		}


		// -----------------------------------------------------------------
		#region Properties

		/// <summary> Event fired when destroying this package. </summary>
		public UnityEvent OnDestroy => onDestroy;

		#endregion
	}
}
