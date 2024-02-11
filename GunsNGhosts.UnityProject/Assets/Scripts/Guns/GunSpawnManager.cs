using UnityEngine;

namespace GunsNGhosts.Guns
{
	/// <summary> Controls the chances to drop each gun type. </summary>
	public class GunSpawnManager : MonoBehaviour
	{
		[Space] [SerializeField] GunSpawnChance[] guns = { };


		// -----------------------------------------------------------------

		/// <summary> Return a random Gun based on the spawning chances of each Gun. </summary>
		public Gun GetRandomGun()
		{
			// Sum the total chances of Gun appearnce.
			float sum = 0;
			foreach(GunSpawnChance gun in guns)
				sum += gun.chance;

			// Get a random num whitin that sum.
			float randomNum = Random.Range(0f, sum);

			// Return the Gun with that random number.
			sum = 0;
			foreach (GunSpawnChance gun in guns)
			{
				sum += gun.chance;
				if (sum > randomNum)
					return gun.gun;
			}
			return null;
		}


		// -----------------------------------------------------------------

		/// <summary> Reference to a Gun and its chances of appearing. </summary>
		[System.Serializable]
		public class GunSpawnChance
		{
			public Gun gun = null;
			public float chance = 1;
		}
	}
}
