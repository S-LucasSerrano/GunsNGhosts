using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GunsNGhosts.Damage;

namespace GunsNGhosts.UI
{
	public class HealthBar : MonoBehaviour
	{
		/// <summary> Image that will show the ammount of health. </summary>
		[Space] [SerializeField] Image bar = null;
		/// <summary> Animator of the health bar. </summary>
		[SerializeField] Animator animator = null;
		/// <summary> Reference to health component of the player. </summary>
		PlayerHealth healthComponent = null;


		// ----------------------------------------------------------------------

		private IEnumerator Start()
		{
			// Find a reference to the health component in the scene.
			healthComponent = Game.Player.Health;
			// Add a listener to the damage event.
			healthComponent.OnDamaged.AddListener(OnPlayerDamage);

			// Wait one frame so the health component has ended its Start call, and show the starting health.
			yield return null;
			bar.fillAmount = (float)healthComponent.CurrentHealth / (float)healthComponent.MaxHealth;
		}


		// ----------------------------------------------------------------------
		public void OnPlayerDamage(int remainingHealth)
		{
			animator.SetTrigger("Damage");
			bar.fillAmount = (float)remainingHealth / (float)healthComponent.MaxHealth;
		}
	}
}
