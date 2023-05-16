using UnityEngine;
using LucasSerrano.Input;

namespace GunsNGhosts.CharacterController
{
	/// <summary> Input manager that saves Movement, Aiming and Shooting inputs. </summary>
	public class TopdownShooterInputManager : InputManagerComponent
	{
		/// <summary> Transform of the character controller. </summary>
		[Space] [SerializeField] Transform playerTransform = null;

		/// <summary> Player's input for movement. </summary>
		Vector2 movementInput = new Vector2();
		/// <summary> Player's input for aiming. </summary>
		Vector2 aimingInput = new Vector2();
		/// <summary> Is the player pressing the shooting input this frame? </summary>
		bool isShooting = false;
		/// <summary> Was the player pressing the shooting input the previous frame? </summary>
		bool wasShooting = false;


		// ----------------------------------------------------------------------
		#region Reset

		private void Reset()
		{
			playerTransform = this.transform;
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Input Manager

		public override float GetAxis(string name)
		{
			switch (name)
			{
				// Movement.
				case "Movement.Horizontal":
					return movementInput.x;
				case "Movement.Vertical":
					return movementInput.y;

				// Aiming.
				case "Aiming.Horizontal":
					return aimingInput.x;
				case "Aiming.Vertical":
					return aimingInput.y;

				default:
					return 0;
			}
		}

		public override bool GetButton(string name)
		{
			switch (name)
			{
				// Shoot
				case "Shoot":
					return isShooting;

				default:
					return false;
			}
		}

		public override bool GetButtonDown(string name)
		{
			switch (name)
			{
				// Shoot
				case "Shoot":
					return (wasShooting == false && isShooting == true);

				default:
					return false;
			}
		}

		public override bool GetButtonUp(string name)
		{
			switch (name)
			{
				// Shoot
				case "Shoot":
					return (wasShooting == true && isShooting == false);

				default:
					return false;
			}
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Update

		private void Update()
		{
			UpdateMovementInput();
			UpdateAimingInput();
			UpdateShootingInput();
		}

		private void LateUpdate()
		{
			wasShooting = isShooting;
		}

		/// <summary> Save Player's input for movement. </summary>
		private void UpdateMovementInput()
		{
			movementInput.x = 0;
			movementInput.y = 0;

			// Arrow keys.
			if (Input.GetKey(KeyCode.LeftArrow))
				movementInput.x -= 1;
			if (Input.GetKey(KeyCode.RightArrow))
				movementInput.x += 1;
			if (Input.GetKey(KeyCode.UpArrow))
				movementInput.y += 1;
			if (Input.GetKey(KeyCode.DownArrow))
				movementInput.y -= 1;

			// WASD keys.
			if (Input.GetKey(KeyCode.A))
				movementInput.x -= 1;
			if (Input.GetKey(KeyCode.D))
				movementInput.x += 1;
			if (Input.GetKey(KeyCode.W))
				movementInput.y += 1;
			if (Input.GetKey(KeyCode.S))
				movementInput.y -= 1;

			movementInput.Normalize();
		}

		/// <summary> Save Player's input for the aiming direction. </summary>
		private void UpdateAimingInput()
		{
			// Mouse.
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			aimingInput = mousePosition - playerTransform.position;

			aimingInput.Normalize();
		}

		/// <summary> Save Player's input for shooting. </summary>
		private void UpdateShootingInput()
		{
			isShooting = Input.GetKey(KeyCode.Mouse0);
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Public

		/// <summary> Player's input for movement. </summary>
		public Vector2 MovementInput => movementInput;

		/// <summary> Player's input for aiming. </summary>
		public Vector2 AimingInput => aimingInput;

		/// <summary> Is the player pressing the shooting input? </summary>
		public bool Shooting => isShooting;

		#endregion
	}
}
