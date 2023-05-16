using UnityEngine;
using LucasSerrano.Input;

namespace GunsNGhosts.CharacterController
{
	/// <summary> Simple controller that moves the character in 360 degrees based on input. </summary>
	public class PlayerMovement : MonoBehaviour, IRequire<Player>
	{
		/// <summary> IputManager that will give us the player input. </summary>
		[Space][SerializeField] InputManagerComponent inputManager;
		/// <summary> Animator of the player character. </summary>
		[Space][SerializeField] Animator animator = null;
		/// <summary> Speed at which the player moves. </summary>
		[Space][SerializeField] float movementSpeed = 5;


		// ----------------------------------------------------------------------
		#region Reset

		public void SetRequirement(Player requirement)
		{
			inputManager = requirement.InputManager;
			animator = requirement.Animator;
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Update

		private void FixedUpdate()
		{
			MovementUpdate();
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Movement

		Vector2 _movementInput = new Vector2();

		/// <summary> Move in the direction of the Player's input. </summary>
		private void MovementUpdate()
		{
			_movementInput.x = inputManager.GetAxis("Movement.Horizontal");
			_movementInput.y = inputManager.GetAxis("Movement.Vertical");
			Move(_movementInput);

			// Update walking animation.
			bool moving = (_movementInput.x != 0 || _movementInput.y != 0);
			if (animator.GetBool("Walk") != moving)
				animator.SetBool("Walk", moving);
		}

		/// <summary> Change position in the given direction. </summary>
		private void Move(Vector2 dir)
		{
			transform.Translate(dir.normalized * movementSpeed * Time.deltaTime);
		}

		#endregion
	}
}
