using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GunsNGhosts.CharacterController;

namespace GunsNGhosts.UI
{    
    /// <summary> Component that show the amount of ammo the player has in the fill ammount of a UI image. </summary>
    public class AmmoBar : MonoBehaviour
    {
		/// <summary> Image that will show the ammount of ammo. </summary>
        [Space] [SerializeField] Image bar = null;

		/// <summary> Reference to the GunShooter component of the player. </summary>
        GunShooter shooter;


		// ----------------------------------------------------------------------

		private IEnumerator Start()
		{
			// Find a reference to the shooter in the scene.
			shooter = ReferenceProvider.GetReference<Player>().Shooter;

			// Wait one frame so the GunShooter has ended its Start call, and show the starting ammo.
			yield return null;
			UpdateAmmoUI();
		}

		private void Reset() => bar = GetComponent<Image>();


		// ----------------------------------------------------------------------

		int prevAmmo = 0;

		private void Update()
		{
			if (shooter == null)
				return;

			// Update the ammo ui while the player is shooting.
			if (shooter.Ammo != prevAmmo)
				UpdateAmmoUI();
			prevAmmo = shooter.Ammo;
		}

		public void UpdateAmmoUI()
		{
			bar.fillAmount = (float)shooter.Ammo / (float)shooter.MaxAmmo;
		}
	}
}
