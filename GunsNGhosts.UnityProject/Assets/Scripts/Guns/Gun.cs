using UnityEngine;

namespace GunsNGhosts.Guns
{
	/// <summary> Base class for all Guns. </summary>
	public abstract class Gun : MonoBehaviour
	{
		/// <summary> Shooter that is shooting this Gun. </summary>
		private IGunShooter shooter = null;
		/// <summary> Is the player shooting? </summary>
		private bool shooting = false;

		/// <summary> Amount of ammo this Gun uses. </summary>
		[Space] [SerializeField] int ammo = 1;
		/// <summary> The sprite for this Gun. </summary>
		[Space] [SerializeField] Sprite sprite = null;
		/// <summary> Name to appear on the UI. </summary>
		[SerializeField] string displayName = "Gun";
		/// <summary> Letter to appear on the UI. </summary>
		[SerializeField] string displayLetter = "G";


		// ----------------------------------------------------------------------
		#region Functions called by the Gun shooter

		/// <summary> Function called when this Gun is equiped. </summary>
		public virtual void Equip(IGunShooter shooter)
		{
			if (shooter == null) return;
			this.shooter = shooter;
		}

		/// <summary> Function called when this Gun is unequiped. </summary>
		public virtual void Unequip() { }

		/// <summary> Function to be called by the Gun shooter when the player starts shooting. </summary>
		public void StartShooting()
		{
			shooting = true;
			OnShootingStarts();
		}

		/// <summary> Function to be called by the Gun shooter while the player is shooting. </summary>
		public void Shooting()
		{
			if (shooter == null) return;

			OnShootingMaintained();
		}

		/// <summary> Function to be called by the Gun shooter when the player stops shooting. </summary>
		public  void EndShooting()
		{
			if (shooter == null) return;

			shooting = false;
			OnShootingEnded();
		}

		#endregion


		// ----------------------------------
		#region Virtual Shooting Functions

		/// <summary> Function called once when the player starts shooting. </summary>
		protected virtual void OnShootingStarts() { }

		/// <summary> Function called every frame the player is shooting. </summary>
		protected virtual void OnShootingMaintained() { }

		/// <summary> Function called once when the player stops shooting. </summary>
		protected virtual void OnShootingEnded() { }

		#endregion


		// ----------------------------------------------------------------------
		#region Properties

		/// <summary> Shooter that is shooting this Gun. </summary>
		protected IGunShooter Shooter => shooter;

		/// <summary> TRUE while the player is shooting. </summary>
		protected bool IsShooting => shooting;

		/// <summary> Amount of ammo this Gun uses per shot. </summary>
		protected int Ammo => ammo;

		/// <summary> Transform that defines the origin and direction of bullets. </summary>
		protected Transform ShootingPoint => shooter.ShootingPoint;

		/// <summary> The sprite to show while this Gun is equiped. </summary>
		public Sprite Sprite => sprite;

		/// <summary> Name to appear on the UI. </summary>
		public string DisplayName => displayName;

		/// <summary> Letter to appear on the UI. </summary>
		public string DisplayLetter => displayLetter;

		#endregion
	}
}
