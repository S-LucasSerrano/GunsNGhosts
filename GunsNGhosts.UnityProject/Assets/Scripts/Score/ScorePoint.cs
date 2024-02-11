using UnityEngine;

namespace GunsNGhosts.Score
{
	/// <summary> Adds score when the player touches it. </summary>
	public class ScorePoint : MonoBehaviour
	{
		/// <summary> Score manger. </summary>
		Score scoreManager = null;
		/// <summary> Score that this point adds when collected. </summary>
		[Space] [SerializeField] int score = 1;


		// ----------------------------------------------------------------

		private void Start()
		{
			scoreManager = FindObjectOfType<Score>();
		}

		// ----------------------------------------------------------------

		private void OnTriggerEnter2D(Collider2D other)
		{
			TryToCollectPoint(other.gameObject.layer);
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			TryToCollectPoint(collision.gameObject.layer);
		}

		void TryToCollectPoint(int layer)
		{
			if (layer == LayerMask.NameToLayer("Player") && scoreManager != null)
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
