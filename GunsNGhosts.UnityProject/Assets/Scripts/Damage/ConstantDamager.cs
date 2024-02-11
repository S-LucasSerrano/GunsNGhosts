using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunsNGhosts.Damage
{
    /// <summary> Makes damage to IDamageables while touching them. </summary>
    public class ConstantDamager : MonoBehaviour
    {
        /// <summary> Damage that this damager makes. </summary>
        [Space] public int damage = 1;
        /// <summary> Time between damage ticks. </summary>
        [SerializeField] float damageCooldown = .25f;
        /// <summary> LayerMask that defines what layers can get damage from this damager. </summary>
        [SerializeField] LayerMask layerMask = int.MaxValue;

		/// <summary> Dictionary that link a game object with the routine that its making damage to that gameObject. </summary>
		Dictionary<GameObject, Coroutine> damagingRoutines = new Dictionary<GameObject, Coroutine>();


		// ------------------------------------------------------------

		private void OnDisable()
		{
			foreach(KeyValuePair<GameObject, Coroutine> routine in damagingRoutines)
			{
				if (routine.Value != null) StopCoroutine(routine.Value);
			}
			damagingRoutines.Clear();
		}


		// ------------------------------------------------------------
		#region Trigger / Collision

		private void OnTriggerEnter2D(Collider2D other)
		{
			MakeDamage(other);
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			MakeDamage(collision.collider);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			StopMakingDamage(other);
		}

		private void OnCollisionExit2D(Collision2D other)
		{
			StopMakingDamage(other.collider);
		}

		#endregion


		// ------------------------------------------------------------
		#region Damage

		void MakeDamage(Collider2D other)
		{
			if (layerMask.ContainsLayer(other.gameObject.layer) == false)
				return;
			if (damagingRoutines.ContainsKey(other.gameObject))
				return;

			IDamageable damageable = other.GetComponent<IDamageable>();
			if (damageable == null)
				return;

			damagingRoutines.Add(other.gameObject, StartCoroutine(DamagingRoutine(damageable)));
		}

		void StopMakingDamage(Collider2D other)
		{
			if (damagingRoutines.ContainsKey(other.gameObject) == false)
				return;			

			StopCoroutine( damagingRoutines[other.gameObject] );
			damagingRoutines.Remove(other.gameObject);
		}

		IEnumerator DamagingRoutine(IDamageable target)
		{
			while(true)
			{
				target.TakeDamage(damage, this);
				yield return new WaitForSeconds(damageCooldown);
			}
		}

		#endregion
	}
}
