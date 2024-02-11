using UnityEngine;

namespace GunsNGhosts.Guns
{
	/// <summary> The Sniper is a <see cref="SimpleGun"/> that activates a laser in front of the player when equiped. </summary>
	public class Sniper : SimpleGun
	{
		[Space] [SerializeField] GameObject laser = null;

		public override void Equip(IGunShooter shooter)
		{
			base.Equip(shooter);

			laser.transform.parent = ShootingPoint;
			laser.transform.position = ShootingPoint.position;
			laser.transform.rotation = ShootingPoint.rotation;

			laser.SetActive(true);
		}

		public override void Unequip()
		{
			laser.SetActive(false);
		}
	}
}
