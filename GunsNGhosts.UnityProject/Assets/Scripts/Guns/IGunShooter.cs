using UnityEngine;

namespace GunsNGhosts.Guns
{
    /// <summary> Functions and properties that Guns need from the Shooter. </summary>
    public interface IGunShooter
    {
        /// <summary> Transforms that defines the origin and direction of bullets. </summary>
        public Transform ShootingPoint { get; }

        /// <summary> Current amount of ammo the player has. </summary>
        public int Ammo { get; }

        /// <summary> Use an ammount of ammo, returning TRUE if the player has enough to shot. </summary>
        public bool UseAmmo(int amount);
    }
}
