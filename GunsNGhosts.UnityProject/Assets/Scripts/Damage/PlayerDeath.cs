using System.Collections;
using UnityEngine;

namespace GunsNGhosts.Damage
{
    /// <summary> When the player dies, activates a GameObject. </summary>
    public class PlayerDeath : MonoBehaviour, IRequire<Player>
    {
        [Space][SerializeField] float delay = 0;
        [SerializeField] GameObject deathUI = null;


		private void Start()
		{
            deathUI.SetActive(false);
		}

		public void SetRequirement(Player requirement)
		{
            requirement.Health.OnDeath.AddListener(OnPlayerDeath);
		}

		void OnPlayerDeath()
        {
            Game.Instance.StartCoroutine(DeathSequence());
        }

        IEnumerator DeathSequence()
		{
            yield return new WaitForSeconds(delay);
            deathUI.SetActive(true);
        }
    }
}
