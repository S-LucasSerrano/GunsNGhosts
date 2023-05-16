using UnityEngine;

namespace GunsNGhosts.Damage
{
	/// <summary> Interface for components that can be damaged. </summary>
	public interface IDamageable
	{
		/// <summary> Make damage to this object. </summary>
		/// <param name="damage"> Damage to make to this object. </param>
		/// <param name="source"> Component that is making the damage. </param>
		public void TakeDamage(int damage, Component source = null);
	}
}
