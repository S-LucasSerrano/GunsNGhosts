using UnityEngine;

namespace GunsNGhosts.Damage
{
	/// <summary> Makes damage to IDamageables when touching them. </summary>
	public class Damager : MonoBehaviour
	{
		/// <summary> Damage that this damager makes. </summary>
		[Space] [SerializeField] int damage = 1;
		/// <summary> LayerMask that defines what layers can get damage from this damager. </summary>
		[SerializeField] LayerMask layerMask = int.MaxValue;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (layerMask.ContainsLayer(other.gameObject.layer) == false)
				return;

			IDamageable damageable = other.GetComponent<IDamageable>();
			if (damageable == null)
				return;

			damageable.TakeDamage(damage, this);
		}
	}
}
