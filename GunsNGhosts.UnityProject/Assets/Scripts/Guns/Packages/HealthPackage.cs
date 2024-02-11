using UnityEngine;
using GunsNGhosts.Damage;

namespace GunsNGhosts.Guns
{
	/// <summary> When added to a package, heals the Player when the package is destroyed. </summary>
	[RequireComponent(typeof(GunPackage))]
	public class HealthPackage : MonoBehaviour
	{
		/// <summary> Percentage of ammo that this packages gives to the player. </summary>
		[Space] [SerializeField] int health = 1;

		PlayerHealth playerHealth = null;


		private void Start()
		{
			var package = GetComponent<GunPackage>();
			package.OnDestroy.AddListener(OnPackageDestroyed);

			playerHealth = ReferenceProvider.GetReference<Player>().Health;
		}

		void OnPackageDestroyed()
		{
			playerHealth.Heal(health);
		}
	}
}
