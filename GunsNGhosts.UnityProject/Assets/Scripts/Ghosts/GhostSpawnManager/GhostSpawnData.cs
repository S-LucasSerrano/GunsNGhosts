using UnityEngine;

namespace GunsNGhosts.Ghosts
{
	/// <summary> Scriptable objet that contains the data that defines how Ghost are spawned based in the player's score. </summary>
    [CreateAssetMenu]
    public class GhostSpawnData : ScriptableObject
    {
		/// <summary> List that defines the chances and intervals of Ghosts spawning for mutiple score values. </summary>
		[Space] public GhostSpawnRange[] spawnRanges = { };

		/// When the score is higher that the one in the last of the ranges above, this spawn data repeats in a loop every X points.
		[Space]  public int repeatEachXPoints = 1000;
		public float repeatMinInterval = 1f;
		public float repeatMaxInterval = 0.1f;
		[Space] public AnimationCurve repeatSpawnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
		[Space] public GhostSpawnChance[] repeatGhostChances = { };
	}
}
