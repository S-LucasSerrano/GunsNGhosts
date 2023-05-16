using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GunsNGhosts.Score
{
    /// <summary> Manager of the score system. Contains the current score and multiplier. </summary>
    public class Score : MonoBehaviour
    {
        int currentScore = 0;
        int multiplier = 1;

        /// <summary> Multplier points that are in the scene. </summary>
        List<ScorePoint> multiplierPoints = new List<ScorePoint>();
        /// <summary> Number of multiplier points collected by the player. </summary>
        int collectedPoints = 0;

        /// UI texts where the score and multiplier are displayed.
        [Space] [SerializeField] TextMeshProUGUI scoreText = null;
        [SerializeField] TextMeshProUGUI multiplierText = null;


		// ---------------------------------------------------------------

		private void Awake()
		{
            var allPoints = FindObjectsOfType<ScorePoint>();
            foreach (ScorePoint point in allPoints)
			{
                point.SetRequirement(this);

                if (point.GetType() == typeof(MultiplierPoint))
                    multiplierPoints.Add(point);
			}

            UpdateUI();
        }



		// ---------------------------------------------------------------

        /// <summary> Collect a point, adding score and playing a cool sound. </summary>
        public void CollectPoint(ScorePoint point)
		{
            AddScore(point.Score);

            // If it is a multiplier point, add multiplier when all have been collected.
            if(multiplierPoints.Contains(point))
			{
                collectedPoints++;
                if (collectedPoints >= multiplierPoints.Count)
                    AddMultiplier();
            }

            // To Do: Play a sound with a highier pitch each time until you stay without picking points for a small time.
		}

        /// <summary> Add score to score count. </summary>
        public void AddScore(int score, bool useMultiplier = true)
		{
            int mul = useMultiplier ? multiplier : 1;
            currentScore += score * mul;
		}

        /// <summary> Add one to the multiplier, reseting all the multiplier points. </summary>
        public void AddMultiplier()
		{
            multiplier++;

            foreach (ScorePoint point in multiplierPoints)
                point.gameObject.SetActive(true);
            collectedPoints = 0;
		}


		// ---------------------------------------------------------------
		#region Lose a point per second

		float timeCounter = 0;

		private void Update()
		{
            timeCounter += Time.deltaTime;
            if (timeCounter > 1)
            {
                currentScore--;
                timeCounter = 0;

                UpdateUI();
            }
        }

        #endregion


        // ---------------------------------------------------------------

        /// <summary> Show the score and multiplier in the UI. </summary>
        void UpdateUI()
        {
            scoreText.text = currentScore.ToString();
            multiplierText.text = "x" + multiplier;
        }
    }
}
