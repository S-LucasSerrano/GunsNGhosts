using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor tool that allows to load every scene in the assets folder from a single window. </summary>
public class EditorSceneLoader : EditorWindow
{
	// -----------------------------------------------------------------
	#region Open Window

	[MenuItem("My Tools/Scene Loader")]
	public static void OpenWindow()
	{
		EditorWindow.GetWindow(typeof(EditorSceneLoader), false, "Scene Loader");		
	}

	#endregion


	// -----------------------------------------------------------------
	#region On GUI

	private void OnGUI()
	{
		EditorGUILayout.Space();
		GUILayout.Label("\t Scene To Load: \n -------------------------------");
		EditorGUILayout.Space();

		DrawSceneSelection();
	}

	Vector2 scrollPosition = Vector2.zero;

	/// <summary> Encuentra todas las escenas de la carpeta de Assets y dibuja un boton por cada una. </summary>
	void DrawSceneSelection()
	{
		// Crear el estilo con el que dibujar los botones.
		GUIStyle loadButtonStyle = LoadButtonStyle();
		GUIStyle playButtonStyle = PlayButtonStyle();

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width( position.width ), GUILayout.Height(position.height - EditorGUIUtility.singleLineHeight * 3 ));

		// Encuentra todas las escenas de la carpeta de Assets y dibuja un boton por cada una.
		string[] searchingPaths = { "Assets" };
		string[] guids = AssetDatabase.FindAssets("t:Scene", searchingPaths);
		foreach(string guid in guids)
		{
			string scenePath = AssetDatabase.GUIDToAssetPath(guid);
			string fileName = ExtractFileNameFromPath(scenePath, ".unity");
			string buttonLabel = fileName + "     (" + scenePath + ")";

			// Poner un Label con el nombre de la escena. En amarillo si se trata de la escena en la que estamos.
			if (EditorSceneManager.GetActiveScene() == EditorSceneManager.GetSceneByPath(scenePath))
				GUI.contentColor = Color.yellow;
			GUILayout.Label("     " + fileName);

			// Dibujar los botones. Desactivados si son los de la escena actual.
			GUILayout.BeginHorizontal();
			{
				if (EditorSceneManager.GetActiveScene() == EditorSceneManager.GetSceneByPath(scenePath))
					GUI.enabled = false;
				if (GUILayout.Button(buttonLabel, loadButtonStyle))     // LOAD
				{
					LoadScene(scenePath);
				}

				GUI.enabled = true;
				GUI.contentColor = Color.white;

				if (GUILayout.Button("Play", playButtonStyle))          // PLAY
				{
					if (LoadScene(scenePath))
					{
						EditorApplication.isPlaying = true;
						EditorApplication.isPaused = false;
					}
				}
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}

		EditorGUILayout.EndScrollView();
	}

	/// <summary> Cargar la escena indicada. El string tiene que tener un aspecto como: "Assets/.../SceneName.unity". </summary>
	/// <returns> TRUE si se ha cargado la escena con exito. </returns>
	bool LoadScene(string scene)
	{
		// Si ya estamos en play cargar la escena por la via del PlayMode.
		if (UnityEditor.EditorApplication.isPlaying)
		{
			SceneManager.LoadScene(scene);
			return true;
		}
		// Preguntar si guardar la escena, y no hacer nada si se le da a Cancelar.
		if (EditorSceneManager.GetActiveScene().isDirty)
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
				return false;
		
		EditorSceneManager.OpenScene(scene);
		return true;
	}

	#endregion

	#region Utils

	/// <summary> Devuelve el GUIStyle de los botones para cargar la escena. </summary>
	GUIStyle LoadButtonStyle()
	{
		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.alignment = TextAnchor.MiddleLeft;
		style.fontStyle = FontStyle.Italic;
		style.fixedWidth = EditorGUIUtility.currentViewWidth / 1.4f;
		return style;
	}

	/// <summary> Devuelve el GUIStyle de los botones de Play. </summary>
	GUIStyle PlayButtonStyle()
	{
		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.fixedWidth = EditorGUIUtility.currentViewWidth / 4f;
		return style;
	}

	/// <summary>
	/// Devuelve el nombre de un archivo dado el path y la extension. </summary>
	string ExtractFileNameFromPath(string path, string extension)
	{
		string[] strs = path.Split('/');										/// Separa el path por "/".
		string fileName = strs[strs.Length - 1];								/// El ultimo es el nombre del archivo.
		fileName = fileName.Substring(0, fileName.Length - extension.Length);   /// Extraer la extension y listo.
		return fileName;
	}

	#endregion
}
