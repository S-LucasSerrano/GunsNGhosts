using UnityEngine;
using GunsNGhosts.CharacterController;

namespace GunsNGhosts.Guns
{
	/// <summary> When added to a package, reloads the Player's Ammo when the package is destroyed. </summary>
	[RequireComponent(typeof(GunPackage))]
	public class AmmoPackage : MonoBehaviour
	{
		/// <summary> Percentage of ammo that this packages gives to the player. </summary>
		[Space] [SerializeField] [Range(0, 1)] float ammo = 1;

		GunShooter shooter = null;


		private void Start()
		{
			var package = GetComponent<GunPackage>();
			package.OnDestroy.AddListener(OnPackageDestroyed);

			shooter = ReferenceProvider.GetReference<Player>().Shooter;
		}

		void OnPackageDestroyed()
		{
			int a = Mathf.RoundToInt(shooter.MaxAmmo * ammo);
			shooter.Reload(a);
		}
	}
}
