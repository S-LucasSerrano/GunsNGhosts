using UnityEngine;

namespace GunsNGhosts.Guns
{
	/// <summary> Base class for Bullets. </summary>
	public abstract class Bullet : MonoBehaviour
	{
		/// <summary> Shoot this Bullet. </summary>
		public virtual void Shoot() { }

		/// <summary> Destroy this Bullet. </summary>
		public virtual void Destroy() { }
	}
}
