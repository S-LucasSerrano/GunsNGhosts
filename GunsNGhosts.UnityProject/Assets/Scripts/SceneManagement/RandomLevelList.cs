using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GunsNGhosts.LevelManagement
{
	/// <summary> Scriptable object that can return a random scene that is different from the active one. </summary>
	[CreateAssetMenu]
	public class RandomLevelList : ScriptableObject
	{
		/// <summary> List of posible scenes that this object can return. </summary>
		[Space] [SerializeField] List<string> sceneNames = new();


		/// <summary> Return a random scene from this object's list of possible scenes. </summary>
		public string GetRandomScene()
		{
			// Create a list that contains every scene in the list except the one that is currently active.
			List<string> otherScenes = new();
			string currentScene = SceneManager.GetActiveScene().name;
			foreach (string scene in sceneNames)
			{
				if (scene != currentScene)
				{
					otherScenes.Add(scene);
				}
			}

			// Return a random scene from that list.
			int randomIndex = Random.Range(0, otherScenes.Count);
			return otherScenes[randomIndex];
		}

		[ContextMenu("Print Random Scene Name")]
		public void PrintRandomScene()
		{
			string randomScene = GetRandomScene();
			Debug.Log(randomScene);
		}
	}
}
