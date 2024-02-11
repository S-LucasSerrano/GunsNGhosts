using UnityEngine;
using TMPro;
using GunsNGhosts.Damage;

namespace GunsNGhosts.Score
{
	/// <summary> When the player dies, this component writes the score to the UI and saves it in player prefs. </summary>
	public class FinalScoreRecorder : MonoBehaviour
	{
		Score score = null;

		/// UI text where we will write the final score.
		[Space] [SerializeField] TextMeshProUGUI scoreText = null;
		[SerializeField] TextMeshProUGUI highScoreText = null;
		/// <summary> GameObject activated when the players makes a new high score. </summary>
		[Space] [SerializeField] GameObject newHighGameObj = null;

		/// Key to save the highscore to the player orefs.
		const string highScoreKey = "HighScore";


		private void Start()
		{
			score = ReferenceProvider.GetReference<Score>();

			PlayerHealth health = ReferenceProvider.GetReference<Player>().Health;
			health.OnDeath.AddListener(OnPlayerDeath);
		}

		void OnPlayerDeath()
		{
			string key = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			key += "_" + highScoreKey;

			int currentScore = score.CurrentScore;
			int highScore = PlayerPrefs.GetInt(key);

			if (currentScore > highScore || highScore == 0)
			{
				highScore = currentScore;
				PlayerPrefs.SetInt(key, highScore);
				
				newHighGameObj.SetActive(true);
			}
			else
			{
				newHighGameObj.SetActive(false);
			}

			scoreText.text = "YOUR SCORE: " + currentScore.ToString();
			highScoreText.text = "HIGH SCORE: " + highScore.ToString();
		}
	}
}
