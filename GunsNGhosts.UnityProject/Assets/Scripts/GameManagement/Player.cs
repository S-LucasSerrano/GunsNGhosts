using UnityEngine;
using LucasSerrano.Input;
using GunsNGhosts.CharacterController;
using GunsNGhosts.Damage;

namespace GunsNGhosts
{
	using PlayerComponent = IRequire<Player>;

	/// <summary> Represent the player character. With reference to some of its components. </summary>
	public class Player : MonoBehaviour
	{
		[Space]
		[SerializeField] InputManagerComponent inputManager = null;

		[Space]
		[SerializeField] GunShooter shooter = null;
		[SerializeField] PlayerHealth health = null;

		[Space]
		[SerializeField] Animator animator = null;


		// ----------------------------------------------------------------------

		private void Reset()
		{
			shooter = GetComponentInChildren<GunShooter>();
			animator = GetComponentInChildren<Animator>();
		}

		private void Awake()
		{
			// Find all components in this GameObject that need a reference to this Player and assign them.
			PlayerComponent[] components = GetComponentsInChildren<PlayerComponent>();
			foreach (PlayerComponent component in components)
				component.SetRequirement(this);
		}


		// ----------------------------------------------------------------------
		#region Properties

		/// <summary> Player's GameObject. </summary>
		public GameObject GameObject => gameObject;

		/// <summary> Player's Tramsform. </summary>
		public Transform Transform => transform;

		/// <summary> InputManager that control the player's input. </summary>
		public InputManagerComponent InputManager => inputManager;

		/// <summary> Player's GunShooter component. </summary>
		public GunShooter Shooter => shooter;

		/// <summary> Player's health component. </summary>
		public PlayerHealth Health => health;

		/// <summary> Player's character animator. </summary>
		public Animator Animator => animator;

		#endregion
	}
}
