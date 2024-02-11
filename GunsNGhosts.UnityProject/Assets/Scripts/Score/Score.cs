using System.Collections;
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
        // Animators of the UI texts.
        Animator scoreAnimator = null;
        Animator multiplierAnimator = null;

        /// <summary> AudioSource played when collecting points. </summary>
        [Space] [SerializeField] AudioSource pointSoundEffect = null;
        /// <summary> Pitch increased each time a point is collected. </summary>
        [SerializeField] float pitchIncrease = 0.1f;
        /// <summary> Max pitch of the point sound effect. </summary>
        [SerializeField] float maxPitch = 2;
        /// <summary> If you spend this time whitout taking points, the pitch of the point sound effect is reseted. </summary>
        [SerializeField] float pitchResetTime = 0.5f;
        /// <summary> Original pitch of the point sound effect. </summary>
        float originalPitch = 1;
        /// <summary> Coroutine that is waiting the time to reset the pitch. </summary>
        Coroutine pitchResetRoutine = null;

        /// <summary> AudioSource played when adding multiplier. </summary>
        [Space] [SerializeField] AudioSource multiplierSoundEffect = null;

        /// <summary> Particles played when the player collects a point. </summary>
        [Space] [SerializeField] ParticleSystem pointParticles = null;


		// ---------------------------------------------------------------

		private void Awake()
		{
            scoreAnimator = scoreText.GetComponent<Animator>();
            multiplierAnimator = multiplierText.GetComponent<Animator>();

            originalPitch = pointSoundEffect.pitch;

            // Find all multiplier points in the scene.
            var points = FindObjectsOfType<MultiplierPoint>();
            foreach (MultiplierPoint point in points)
			{
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

            // Animation.
            scoreAnimator.SetTrigger("Add");
            // Play a sound with a highier pitch each time until you stay without picking points for a small time.
            pointSoundEffect.Play();
            if (pointSoundEffect.pitch < maxPitch)
                pointSoundEffect.pitch += pitchIncrease;
            if (pitchResetRoutine != null) StopCoroutine(pitchResetRoutine);
            pitchResetRoutine = StartCoroutine(ResetPitchRoutine());
            // Partciles
            pointParticles.transform.position = point.transform.position;
            pointParticles.Play();
        }

        /// <summary> Corroutine that resets the pitch of the sound after some time. </summary>
        IEnumerator ResetPitchRoutine()
        {
            yield return new WaitForSeconds(pitchResetTime);
            pointSoundEffect.pitch = originalPitch;
        }

        // ------------------------------

        /// <summary> Add score to score count. </summary>
        public void AddScore(int score, bool useMultiplier = true)
		{
            int mul = useMultiplier ? multiplier : 1;
            currentScore += score * mul;

            UpdateUI();
		}

        /// <summary> Add one to the multiplier, reseting all the multiplier points. </summary>
        public void AddMultiplier()
		{
            multiplier++;

            foreach (ScorePoint point in multiplierPoints)
                point.gameObject.SetActive(true);
            collectedPoints = 1;

            UpdateUI();

            multiplierAnimator.SetTrigger("Add");   // Animation.
            multiplierSoundEffect.Play();           // Sound.
        }


		// ---------------------------------------------------------------
		#region Udate

		float timeCounter = 0;

		private void Update()
		{
            // Lose a point each second.
            timeCounter += Time.deltaTime;
            if (timeCounter > 1)
            {
                currentScore -= multiplier;
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

            if (multiplier > 1)
                multiplierText.text = "x" + multiplier;
            else
                multiplierText.text = "";
        }

        // ---------------------------------------------------------------
        #region Properties

        public int CurrentScore => currentScore;

		#endregion
	}
}
