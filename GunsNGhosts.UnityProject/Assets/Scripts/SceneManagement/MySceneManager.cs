using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Componente persistente que controla todas las operaciones de cambio de escena. </summary>
[AddComponentMenu("Scene Package/Scene Manager")]
public class MySceneManager : MonoBehaviour
{
    private static MySceneManager _instance = null;   /// Singleton.

	/// <summary> Componente que hace fade de la pantalla al cambiar de escenas. </summary>
	SceneFader sceneFader = null;

	/// <summary> Nombre de la escena que hace de pantalla de carga. </summary>
	[Space][SerializeField] string loadingScene = "LoadingScreen";
    /// <summary> Nombre de la escena a la que se quiere ir. </summary>
    string targetScene = "";
	/// <summary> El tiempo minimo que se tiene que estar en la pantalla de carga. </summary>
	[Min(0)][SerializeField] float minLoadingTime = 0;

	/// <summary> Bool que indica si se tiene o no que hacer fade out al llegar a una nueva escena. </summary>
	bool useFadeOnLoad = false;


	// ---------------------------------------------
	#region Awake

	private void Awake()
	{
		PersistentSetup();
		FindSceneFader();

		// Añadir un listner a la accion de cargar una escena.
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	/// <summary> Asegurarse de que solo hay una instancia de este componente.
	/// Y de que no se destruye al cambiar de escenas. </summary>
	void PersistentSetup()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
			Destroy(this);
	}

	#endregion

	#region On Scene Loaded

	/// <summary>
	/// Funcion llamada siempre que se carga una nueva escena. </summary>
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Buscar el SceneFader de la escena.
		if (SceneFader == null)
			FindSceneFader();
		// Hacer fade out si toca hacerlo.
		if (sceneFader != null)
		{
			if (useFadeOnLoad)
				sceneFader.Fade(false);
			else
				sceneFader.FadeInmidiatly(false);
		}

		// Si la escena a la que hemos llegado es la escena de carga,
		// Empezar la carga asyncrona de la escena objetivo.
		if (scene.name == loadingScene)
			LoadSceneAsync(targetScene, useFadeOnLoad);

		useFadeOnLoad = false;	/// Resetear el uso del fade para la siguiente carga.
	}

	/// <summary> Busca y guarda una referencia el SceneFader que hay en la escena. </summary>
	void FindSceneFader()
	{
		sceneFader = FindObjectOfType<SceneFader>();
	}

	#endregion


	// ---------------------------------------------
	#region Load Scene

	/// <summary>
	/// Carga una escena directamente, sin hacer fade. </summary>
	/// <param name="useLoadingScreen"> Si se va a hacer usando la pantalla de carga. </param>
	public void LoadScene(string scene, bool useLoadingScreen = false)
	{
		if (!CanLoadScene(scene))
			return;
			
		targetScene = scene;
		if (!useLoadingScreen)
			SceneManager.LoadScene(targetScene);
		else if (CanLoadScene(loadingScene))
			SceneManager.LoadScene(loadingScene);
	}

	#endregion

	#region Fade To Scene

	/// <summary>
	/// Carga una escena usando un fade. </summary>
	/// <param name="useLoadingScreen"> Si se va a hacer usando la pantalla de carga. </param>
	public void FadeToScene(string scene, bool useLoadingScreen = false)
	{
		// Si no se puede cargar la escena o ya se esta haciendo un fade, return.
		if (!CanLoadScene(scene)) return;
		if (sceneFader != null && sceneFader.Fading) return;

		// Indicar que se debe hacer fade out al llegar a la nueva escena.
		useFadeOnLoad = true;       

		// Si en esta escena no hay fader, cargamos la escena sin fade.
		if (sceneFader == null)
		{
			LoadScene(scene, useLoadingScreen);
			return;
		}
		// Cargar la escena con un fade.
		StartCoroutine(FadeToSceneRoutine(scene, useLoadingScreen));
	}

	IEnumerator FadeToSceneRoutine(string scene, bool useLoadingScreen)
	{
		// Empieza un fade y espera a que termine.
		sceneFader.Fade(true);
		while (sceneFader.Fading)
			yield return null;

		LoadScene(scene, useLoadingScreen);
	}

	#endregion

	#region Load Scene Async

	bool _loadingAsync = false;
	float _asyncProgress = 1;

	/// <summary> Carga una escena como un proceso de fondo. </summary>
	/// <param name="fadeToScene">Si se debe hacer fade entre escenas.</param>
	public void LoadSceneAsync(string scene, bool fadeToScene = true)
	{
		if (!CanLoadScene(scene) || _loadingAsync)
			return;
		StartCoroutine(AsyncLoadingRoutine(scene, fadeToScene));
	}

	IEnumerator AsyncLoadingRoutine(string scene, bool fadeToScene)
	{
		_loadingAsync = true;

		/// Esto hay que esperarlo porque AsyncOperation.allowSceneActivation es ignorado en el Awake.				( https://answers.unity.com/questions/1314424/asyncoperationallowsceneactivation-seems-to-be-ign.html )
		/// Y entiendo que como esta funcion es llamada nada mas cargar la loadingScene, estamos en Awake time.		( https://issuetracker.unity3d.com/issues/loadsceneasync-allowsceneactivation-flag-is-ignored-in-awake )
		yield return new WaitForEndOfFrame();

		float timeCounter = 0;
		_asyncProgress = 0;

		// Esperar a que termine un fade si esta teniendo lugar.
		while (sceneFader != null && sceneFader.Fading)
			yield return null;

		// Empieza a cargar la escena de fondo en baja prioridad.
		Application.backgroundLoadingPriority = ThreadPriority.Low;
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

		asyncLoad.allowSceneActivation = false;     /// Esto prohibe cambiar de escena cuando esta lista, parando AsyncOperation.progress en 0.9.
		while (asyncLoad.isDone == false)           /// AsyncOperation.isDone solo es true cuando el progreso ha llegado a 1.
		{
			yield return null;
			timeCounter += Time.deltaTime;
			_asyncProgress = asyncLoad.progress;

			// Cuando la carga esta lista para cambiar de escena.
			if (_asyncProgress == 0.9f)
			{
				// Esperar a que haya pasado al menos el tiempo indicado como minimo.
				while (timeCounter < minLoadingTime)
				{
					timeCounter += Time.deltaTime;
					_asyncProgress = Mathf.Lerp(0.9f, 1, timeCounter / minLoadingTime);

					yield return null;
				}

				// Hacer fade si se ha indicado.
				if (sceneFader != null && fadeToScene)
				{
					sceneFader.Fade(true);
					while (sceneFader.Fading)
						yield return null;

					useFadeOnLoad = true;     /// Indicar si hay que usar fade al llegar a la escena nueva.
				}

				// Permitir activar la siguiente escena.
				asyncLoad.allowSceneActivation = true;
				_loadingAsync = false;
				_asyncProgress = 1;
			}
		}
	}

	#endregion


	// ---------------------------------------------
	#region Utilities

	/// <summary> Devuelve TRUE si es posible cargar la escena. </summary>
	/// E imprime un error en consola con una posible explicacion de que esta fallando.
	bool CanLoadScene(string scene)
	{
		if (Application.CanStreamedLevelBeLoaded(scene))
			return true;
		Debug.LogError("La escena [" + scene + "] no pudo cargarse." +
			"\n ¿Has escrito bien el nombre de la escena? ¿Falta añadirla en las BuildingSettings?");
		return false;
	}

	#endregion


	// ---------------------------------------------
	#region Propiedades

	/// Referencia publica al singleton.
	public MySceneManager Instance => _instance;

	/// <summary> Componente que hace fade de la pantalla al cambiar de escenas. </summary>
	public SceneFader SceneFader => sceneFader;

	/// <summary> Progreso de la carga en segundo plano. De 0 a 1. </summary>
	public float AsyncLoadProgress => _asyncProgress;

	#endregion
}
