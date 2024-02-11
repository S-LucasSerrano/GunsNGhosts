using UnityEngine;

namespace GunsNGhosts.Ghosts
{
	/// <summary> Class that defines the chances and intervals of Ghosts spawning between two score values. </summary>
	[System.Serializable]
	public class GhostSpawnRange
	{
		public string inspectorName = "Spawn Range 0";

		[Space] public int minScore = 0;
		public float intervalAtMinScore = 1f;

		[Space] public int maxScore = 1000;
		public float intervalAtMaxScore = 0.1f;

		[Space] public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

		[Space] public GhostSpawnChance[] ghostChances = { };
	}

	/// <summary> Id of a Ghost and its chances of spawning. </summary>
	[System.Serializable]
	public class GhostSpawnChance
	{
		public string id = "";
		public float chance = 1;
	}
}
