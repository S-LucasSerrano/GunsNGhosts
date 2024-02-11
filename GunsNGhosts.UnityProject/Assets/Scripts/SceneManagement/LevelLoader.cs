using UnityEngine;
using UnityEngine.SceneManagement;

namespace GunsNGhosts.LevelManagement
{
	/// <summary> Loads a level from a <see cref="RandomLevelList"/>. </summary>
	public class LevelLoader : MonoBehaviour
	{
		/// <summary> Reference to the SceneManager. </summary>
		MySceneManager manager = null;

		/// <summary> Reference to the scriptable object that will give us a random scene name to play next. </summary>
		[Space] [SerializeField] RandomLevelList randomSceneList = null;
		/// <summary> Name of the scene that we will make every player play for the first time. </summary>
		[SerializeField] string tutorialLevel = "";


		// ---------------------------------------------

		// Find a reference to the manager on start.
		private void Start() => manager = FindObjectOfType<MySceneManager>();


		// ---------------------------------------------

		static bool randomizeLevels = true;

		/// <summary> Load a new random level to play. </summary>
		public void LoadLevel()
		{
			string targetScene = "";

			// If the player loaded a specific level, always return to it.
			if (randomizeLevels == false)
			{
				targetScene = SceneManager.GetActiveScene().name;
			}
			else
			{
                // If the player has never played the game, load the tutorial level.
                bool played = PlayerPrefs.GetInt("played", 0) > 0;
                targetScene = played == false ? tutorialLevel : randomSceneList.GetRandomScene();
                // Save that the player has played for the first time.
                PlayerPrefs.SetInt("played", 1);
            }

			if (manager)
				manager.FadeToScene(targetScene);
			else
				SceneManager.LoadScene(targetScene);
		}

		public void LoadLevel(string targetScene)
		{
			randomizeLevels = false;

            if (manager)
                manager.FadeToScene(targetScene);
            else
                SceneManager.LoadScene(targetScene);
        }

		public void LoadRandomLevel()
		{
			randomizeLevels = true;
			LoadLevel();
        }
	}
}
