using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GunsNGhosts.CharacterController;
using LucasSerrano.Input;

namespace GunsNGhosts.Guns
{
	/// <summary> Makes sure the player always has a gun package to pick. </summary>
	public class GunInsurance : MonoBehaviour
	{
		InputManagerComponent inputManager;
		IGunShooter shooter;
		Transform playerTransform;

		Coroutine checkRoutine = null;
		[Space] [SerializeField] float checkInterval = 1;

		/// When the player runs out of ammo a package will appear in the position he was this seconds ago.
		[SerializeField] int savePositionInterval = 2;
		/// Counter the we use to know when to save a new position to the list.
		float timeCounter = 0;
		/// List of position the player has been in.
		List<Vector3> playerPositions = new();

		[SerializeField] ParticleSystem spawnParticles = null;


		// -----------------------------------------------------------

		private void Start()
		{
			inputManager = Game.Instance.GetReference<InputManagerComponent>();
			Player player = Game.Instance.GetReference<Player>();
			shooter = player.Shooter;
			playerTransform = player.transform;

			playerPositions.Add(playerTransform.position );
		}

		private void Update()
		{
			// When you shoot the last bullet, start the checking routine to spawn packages if the are none.
			if (inputManager.GetButton("Shoot")
				&& shooter.Ammo <= 0
				&& checkRoutine == null)
			{
				checkRoutine = StartCoroutine(CheckRoutine());
			}

			// Every second we add the player position the list of past positions, removing the oldest one.
			if (timeCounter >= 1)
			{
				timeCounter = 0;

				playerPositions.Add(playerTransform.position);
				if (playerPositions.Count > savePositionInterval)
					playerPositions.RemoveAt(0);
			}
			timeCounter += Time.deltaTime;
		}

		IEnumerator CheckRoutine()
		{
			while (shooter.Ammo <= 0)
			{
				// Check if there are active packages in the scene.
				AmmoPackage[] packages = FindObjectsOfType<AmmoPackage>(false);
				// If there are not, spawn one in the starting position of the player.
				if (packages.Length <= 0)
				{
					GameObject packageGo = Game.GetElementFromPool("package.ammo");
					packageGo.SetActive(true);
					GunPackage package = packageGo.GetComponent<GunPackage>();
					package.Respawn();

					package.transform.position = playerPositions[0];

					spawnParticles.transform.position = package.transform.position;
					spawnParticles.Play();
				}

				yield return new WaitForSeconds(checkInterval);
			}
			checkRoutine = null;
		}
	}
}
