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
			healthComponent = ReferenceProvider.GetReference<Player>().Health;

			// Wait one frame so the health component has ended its Start call, and show the starting health.
			yield return null;
			bar.fillAmount = (float)healthComponent.CurrentHealth / (float)healthComponent.MaxHealth;
		}

		int _lastHP = 0;

		private void Update()
		{
			if (healthComponent.CurrentHealth != _lastHP)
				UpdateUI();

			_lastHP = healthComponent.CurrentHealth;
		}

		public void UpdateUI()
		{
			animator.SetTrigger("Damage");
			bar.fillAmount = (float)healthComponent.CurrentHealth / (float)healthComponent.MaxHealth;
		}
	}
}
