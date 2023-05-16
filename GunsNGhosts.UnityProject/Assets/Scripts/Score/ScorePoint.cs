using UnityEngine;

namespace GunsNGhosts.Score
{
	/// <summary> Adds score when the player touches it. </summary>
	public class ScorePoint : MonoBehaviour, IRequire<Score>
	{
		/// <summary> Score manger. </summary>
		Score scoreManager = null;
		/// <summary> Score that this point adds when collected. </summary>
		[Space] [SerializeField] int score = 1;


		// ----------------------------------------------------------------

		public void SetRequirement(Score requirement)
		{
			scoreManager = requirement;
		}


		// ----------------------------------------------------------------

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Player") 
				&& scoreManager != null)
			{
				scoreManager.CollectPoint(this);
				gameObject.SetActive(false);
			}
		}


		// ----------------------------------------------------------------

		/// <summary> Score that this point adds when collected. </summary>
		public int Score
		{
			get => score;
			set => score = value;
		}
	}
}
