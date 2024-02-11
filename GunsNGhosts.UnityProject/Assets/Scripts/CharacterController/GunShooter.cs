using UnityEngine;
using LucasSerrano.Input;
using GunsNGhosts.Guns;

namespace GunsNGhosts.CharacterController
{
	/// <summary> Controller that can aim in 360 degrees and shoot with the Gun system. </summary>
	public class GunShooter : MonoBehaviour, IRequire<Player>, IGunShooter
	{
		/// <summary> InputManager that will give us the player input. </summary>
		InputManagerComponent inputManager;

		/// <summary> Transform that rotates towards the aiming direction. </summary>
		[Space] [SerializeField] Transform gunPivot = null;
		/// <summary> Sprite of the Gun. </summary>
		[SerializeField] SpriteRenderer gunSprite = null;

		/// <summary> Currently equipped Gun. </summary>
		[Space] [SerializeField] Gun gun = null;
		/// <summary> Transforms that defines the origin and direction of bullets. </summary>
		[SerializeField] Transform shootingPoint = null;
		/// <summary> TRUE while the input for shooting is pressed. </summary>
		bool shooting = false;

		/// <summary> Max ammo the player can have. </summary>
		[Space][SerializeField] int maxAmmo = 50;
		/// <summary> Current ammount of ammo the player has. </summary>
		int ammo = 0;

		/// Animator of the name that appear on top of the player.
		[Space] [SerializeField] Animator gunCanvasAnimator = null;
		/// Texts where the Gun name will be written.
		[SerializeField] TMPro.TextMeshProUGUI[] gunNameText = { };


		// ----------------------------------------------------------------------
		#region Start

		public void SetRequirement(Player requirement)
		{
			inputManager = requirement.InputManager;
		}

		private void Start()
		{
			gunCanvasAnimator.transform.parent = transform.parent;

			Equip(gun, true);
			ammo = maxAmmo;
		}

		private void OnDisable()
		{
			shooting = false;
			gun.EndShooting();
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Update

		private void Update()
		{
			AimingUpdate();
			ShootingUpdate();
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Aiming

		Vector2 _aimingInput = new Vector2();

		/// <summary> Aim in the direction of the Player's input. </summary>
		private void AimingUpdate()
		{
			_aimingInput.x = inputManager.GetAxis("Aiming.Horizontal");
			_aimingInput.y = inputManager.GetAxis("Aiming.Vertical");
			Aim(_aimingInput);
		}

		/// <summary> Put the Gun looking in a given direction. </summary>
		private void Aim(Vector2 dir)
		{
			Vector3 lookingPoint = transform.position;
			lookingPoint.x += dir.x;
			lookingPoint.y += dir.y;
			gunPivot.LookAt(lookingPoint, Vector3.up);

			// Put the Gun sprite in front of the player when aiming down.
			// And behind when aiming up.
			if (dir.y <= 0)
				gunSprite.sortingOrder = 1;
			else if (gunSprite.sortingOrder != -1)
				gunSprite.sortingOrder = -1;
		}

		#endregion

		#region Shoot

		/// <summary> Tell the Gun to shoot based on player's input. </summary>
		private void ShootingUpdate()
		{
			if (gun == null) return;

			if (inputManager.GetButtonDown("Shoot"))
			{
				shooting = true;
				gun.StartShooting();
			}

			if (inputManager.GetButton("Shoot"))
			{
				gun.Shooting();
			}

			if (inputManager.GetButtonUp("Shoot"))
			{
				shooting = false;
				gun.EndShooting();
			}
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Gun

		/// <summary> Equip a new Gun. </summary>
		public void Equip(Gun newGun, bool initializing = false)
		{
			if (gun != null)
				gun.Unequip();

			gun = newGun;

			if (gun != null)
				gun.Equip(this);

			// Change the sprite.
			gunSprite.sprite = gun != null ? gun.Sprite : null;
			// Show the gun name in the UI texts.
			foreach (TMPro.TextMeshProUGUI text in gunNameText)
				text.text = gun != null ? gun.DisplayName : "";

			if (initializing) return;

			// Play the animation of the Gun name on top the player.
			gunCanvasAnimator.transform.position = transform.position;
			gunCanvasAnimator.SetTrigger("Equip");
		}

		public bool UseAmmo(int amount)
		{
			// This shooter allows to shoot if there is at leats some ammo.

			if (ammo <= 0)
				return false;

			ammo -= amount;
			if (ammo < 0) ammo = 0;

			return true;
		}

		/// <summary> Add the given amount of ammo. </summary>
		public void Reload(int amount)
		{
			ammo += amount;
			if (ammo > maxAmmo)
				ammo = maxAmmo;
		}

		/// <summary> Return the current ammo to the max. </summary>
		public void Reload() => ammo = maxAmmo;


		#endregion


		// ----------------------------------------------------------------------
		#region Properties

		/// <summary> TRUE while the input for shooting is pressed. </summary>
		public bool Shooting => shooting;

		/// <summary> Transforms that defines the origin and direction of bullets. </summary>
		public Transform ShootingPoint
		{
			get => shootingPoint;
			set => shootingPoint = value;
		}

		/// <summary> Max ammo the player can have. </summary>
		public int MaxAmmo => maxAmmo;

		/// <summary> Current ammount of ammo the player has. </summary>
		public int Ammo => ammo;

		#endregion
	}
}
