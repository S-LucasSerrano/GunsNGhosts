using UnityEngine;

namespace GunsNGhosts.Damage
{
	/// <summary> Basic health component. </summary>
	public class Health : MonoBehaviour, IDamageable
	{
		/// <summary> Maximum health value of this component. </summary>
		[Space] [SerializeField] protected int maxHealth = 6;
		/// <summary> Currrent amount of health. </summary>
		protected int currentHealth = 0;


		// ---------------------------------------------------------------

		protected virtual void Start()
		{
			currentHealth = maxHealth;
		}

		// ---------------------------------------------------------------
		#region IDamageable

		public virtual void TakeDamage(int damage, Component source = null)
		{
			if (currentHealth <= 0)
				return;

			currentHealth -= damage;

			if (currentHealth > maxHealth)
				currentHealth = maxHealth;

			if (currentHealth > 0)
				Damaged(damage, source);
			else
				Die(source);
		}

		#endregion


		// ---------------------------------------------------------------
		#region Inheritance

		/// <summary> Function called after taking damage. </summary>
		/// <param name="damage"> Damage to make to this object. </param>
		/// <param name="source"> Component that is making damage to this object. </param>
		protected virtual void Damaged(int damage, Component source = null) { }

		/// <summary> Funcion called when health reaches 0. </summary>
		/// <param name="source"> Component that is making damage to this object. </param>
		protected virtual void Die(Component source = null) { }

		#endregion


		// ---------------------------------------------------------------
		#region Properties

		/// <summary> Maximum health value of this component. </summary>
		public int MaxHealth
		{
			get => maxHealth;
			set => maxHealth = value;
		}

		/// <summary> Currrent amount of health. </summary>
		public int CurrentHealth => currentHealth;

		#endregion
	}
}

