using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunsNGhosts.Ghosts
{
	public class GhostSpawnManager : MonoBehaviour
	{
		GunsNGhosts.Score.Score scoreManager = null;

		[Space] [SerializeField] float minRadius = 0;
		[SerializeField] float maxRadius = 0;

		/// Data that defines how Ghost are spawned based on the player's score.
		[Space] [SerializeField] GhostSpawnData spawnData = null;

		/// Properties that give easy access to the spawn data. 
		GhostSpawnRange[] spawnRanges => spawnData.spawnRanges;
		int repeatEachXPoints => spawnData.repeatEachXPoints;
		float repeatMinInterval => spawnData.repeatMinInterval;
		float repeatMaxInterval => spawnData.repeatMaxInterval;
		AnimationCurve repeatSpawnCurve => spawnData.repeatSpawnCurve;
		GhostSpawnChance[] repeatGhostChances => spawnData.repeatGhostChances;

		Coroutine _spawningRoutine = null;


		// ---------------------------------------------------------------

		void Start()
		{
			scoreManager = ReferenceProvider.GetReference<GunsNGhosts.Score.Score>();
			ReferenceProvider.GetReference<Player>().Health.OnDeath.AddListener( StopSpawiningRoutine );

			_spawningRoutine = StartCoroutine(SpawningRoutine());
		}

		void StopSpawiningRoutine()
		{
			if (_spawningRoutine != null)
				StopCoroutine(_spawningRoutine);
		}

		IEnumerator SpawningRoutine ()
		{
			while (true)
			{
				// Calculate the spawn range we are currently in based on player score.
				GhostSpawnRange currentSpawnRange = GetCurrentSpawnRange();

				// If the current score is above the last range, loop between the repeating chances.
				if (currentSpawnRange == null)
				{
					// Spawn a Ghost.
					SpawmRandomGhost(repeatGhostChances);

					// Calculate how above we are of the last range.
					GhostSpawnRange lastSpawnChance = spawnRanges[spawnRanges.Length - 1];
					int scoreDiff = scoreManager.CurrentScore - lastSpawnChance.maxScore;
					// We subtract the spawiningLoopScore from that diff
					// until we have a number that is between 0 and the spawningLoopScore itself.
					while (scoreDiff > repeatEachXPoints)
						scoreDiff -= repeatEachXPoints;

					// We normalize that value to have 0 if it was 0 or 1 if it was exactly spawningLoopScore,
					float t = (float)scoreDiff / (float)repeatEachXPoints;
					// And use to calculate the calculate the time to wait before spawning the next Ghost
					// using the indicared curve and interval values.
					t = repeatSpawnCurve.Evaluate(t);
					t = Utilities.Math.Remap(t, 0, 1, repeatMinInterval, repeatMaxInterval);

					yield return new WaitForSeconds(t);
					continue;
				}

				// Spawn a random Ghost with the range chances.
				SpawmRandomGhost(currentSpawnRange.ghostChances);

				// Wait to respawn the next Ghost based on the current player's score.
				int score = scoreManager.CurrentScore;
				score = Mathf.Clamp(score, currentSpawnRange.minScore, currentSpawnRange.maxScore);

				float wait = Utilities.Math.Remap(score, currentSpawnRange.minScore, currentSpawnRange.maxScore, 0, 1);
				wait = currentSpawnRange.curve.Evaluate(wait);
				wait = Utilities.Math.Remap(wait, 0, 1, currentSpawnRange.intervalAtMinScore, currentSpawnRange.intervalAtMaxScore);

				yield return new WaitForSecondsRealtime(wait);
			}
		}

		/// <summary> Calculate the spawn range we are currently in based on player score. </summary>
		GhostSpawnRange GetCurrentSpawnRange()
		{
			foreach(GhostSpawnRange range in spawnRanges)
			{
				if (scoreManager.CurrentScore < range.maxScore)
					return range;
			}
			return null;
		}

		/// <summary> Spawn a random ghost from a list of ghost chances. </summary>
		void SpawmRandomGhost(GhostSpawnChance[] ghostChances)
		{
			string ghostId = "";
			
			// Sum the total chances of Ghost appearnce.
			float sum = 0;
			foreach (GhostSpawnChance ghost in ghostChances)
				sum += ghost.chance;
			// Get a random num whitin that sum.
			float randomNum = Random.Range(0f, sum);

			// Use the id of the Ghost with that random number.
			sum = 0;
			foreach (GhostSpawnChance ghost in ghostChances)
			{
				sum += ghost.chance;
				if (sum > randomNum)
				{
					ghostId = ghost.id;
					break;
				}
			}

			if (string.IsNullOrEmpty(ghostId))
				return;

			// Spawn.
			GameObject newGhost = Game.GetElementFromPool(ghostId);
			newGhost.SetActive(true);

			// Place it in a random position.
			Vector3 targetPos = Vector3.zero;
			targetPos.x = Random.Range(-1f, 1f);
			targetPos.y = Random.Range(-1f, 1f);

			float randomDis = Random.Range(0, 1);
			randomDis = Utilities.Math.Remap(randomDis, 0, 1, minRadius, maxRadius);
			targetPos = targetPos.normalized * randomDis;

			newGhost.transform.position = transform.position + targetPos;
		}


		// ---------------------------------------------------------------

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, minRadius);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, maxRadius);
		}
	}
}
