using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;


/// <summary>
/// Componente que hace de intermediario entre la escena y el IsosSceneManager (que es persistente). </summary>
/// 
/// En otros proyectos esta funcion la cumple SceneGeneric.
/// 
[AddComponentMenu("Scene Package/Scene Loader")]
public class SceneLoader : MonoBehaviour
{
	MySceneManager manager = null;

	/// <summary> Si tiene o no que cargar la siguiente escena automaticamente en el Start(). </summary>
	[Space][SerializeField] bool loadOnStart = false;
	/// <summary> El nombre de la escena a cargar automaticamente. </summary>
	[SerializeField] string targetScene = "";
	/// <summary> El tiempo que espera antes de cargar la escena automaticamente. </summary>
	[Min(0)][SerializeField] float delay = 0;
	/// <summary> Si tiene o no que hacer fade antes de cargar otra escena. </summary>
	[SerializeField] bool useFade = true;
	/// <summary> Si tiene o no que pasar por la pantalla de carga antes de cargar la escena automaticamente. </summary>
	[SerializeField] bool useLoadingScreen = true;


	// ---------------------------------------------
	#region Start

	private void Start()
	{
		// Busca el manager.
		manager = FindObjectOfType<MySceneManager>();

		// Si se ha indicado, cargar la escena automaticamente.
		if (loadOnStart	&& !string.IsNullOrWhiteSpace(targetScene))
			StartCoroutine(LoadWithDelay());
	}

	/// <summary> Corrutina que carga la escena indicada en el inspector. </summary>
	IEnumerator LoadWithDelay()
	{
		// Esperar el tiempo indicado.
		if (delay > 0)
			yield return new WaitForSecondsRealtime(delay);
		else
			yield return new WaitForEndOfFrame();

		// Cargar con las opciones indicadas.
		if (useFade)
		{
			if (useLoadingScreen)
				FadeToSceneAsync(targetScene);
			else
				FadeToScene(targetScene);
		}
		else
		{
			if (useLoadingScreen)
				LoadSceneAsync(targetScene);
			else
				LoadScene(targetScene);
		}
	}

	#endregion


	// ---------------------------------------------
	#region Funciones publicas: Load Scene

	/// <summary>
	/// Llama al IsosSceneManager para que cargue la escena indicada directamente. </summary>
	public void LoadScene(string scene)
	{
		if (manager != null)
				manager.LoadScene(scene, false);
		else
			LoadSceneWithoutManager(scene);
	}

	/// <summary>
	/// Llama al IsosSceneManager para que cargue la escena indicada pasando por la pantalla de carga. </summary>
	public void LoadSceneAsync(string scene)
	{
		if (manager != null)
			manager.LoadScene(scene, true);
		else
			LoadSceneWithoutManager(scene);
	}
	
	/// <summary>
	/// Llama al IsosSceneManager para que cargue la escena indicada con un fade. </summary>
	public void FadeToScene(string scene)
	{
		if (manager != null)
			manager.FadeToScene(scene);
		else
			LoadSceneWithoutManager(scene);
	}

	/// <summary>
	/// Llama al IsosSceneManager para que cargue la escena indicada con un fade y la pantalla de carga. </summary>
	public void FadeToSceneAsync(string scene)
	{
		if (manager != null)
			manager.FadeToScene(scene, true);
		else
			LoadSceneWithoutManager(scene);
	}

	/// <summary>
	/// Carga la escena objetivo sin mas, sin manager, ni fade, ni nada. </summary>
	void LoadSceneWithoutManager(string scene)
	{
		SceneManager.LoadScene(scene);
	}

	#endregion
}


// ---------------------------------------------------------------------------------------------------------------------------------------
#region Custom Editor
#if UNITY_EDITOR

[CustomEditor(typeof(SceneLoader))]
[CanEditMultipleObjects]
public class SceneLoader_editor : Editor
{
	SerializedProperty loadOnStart;
	SerializedProperty targetScene;
	SerializedProperty delay;
	SerializedProperty useFade;
	SerializedProperty useLoadingScreen;

	public void OnEnable()
	{
		loadOnStart = serializedObject.FindProperty("loadOnStart");
		targetScene = serializedObject.FindProperty("targetScene");
		delay = serializedObject.FindProperty("delay");
		useLoadingScreen = serializedObject.FindProperty("useLoadingScreen");
		useFade = serializedObject.FindProperty("useFade");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		/// Si no se va a cargar en el Start, desactivar los campos que no se van a usar.
		EditorGUILayout.PropertyField(loadOnStart);
		if (!loadOnStart.boolValue)
			GUI.enabled = false;
		EditorGUILayout.PropertyField(targetScene);
		EditorGUILayout.PropertyField(delay);
		EditorGUILayout.PropertyField(useFade);
		EditorGUILayout.PropertyField(useLoadingScreen);
		GUI.enabled = true;

		serializedObject.ApplyModifiedProperties();
	}
}

#endif
#endregion
