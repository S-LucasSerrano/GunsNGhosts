using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Componente que modifica el FillAmount de una imagen de la UI para que sea igual al progreso de carga del IsosSceneManager. </summary>
[AddComponentMenu("Scene Package/Loading Progress Bar")]
public class LoadingProgressBar : MonoBehaviour
{
    MySceneManager manager = null;

    /// <summary> La imagen que va a mostrar el progreso. </summary>
    [Space] public Image loadingBar = null;


	// -----------------------------------------------------------------
	private void Start()
	{
		manager = FindObjectOfType<MySceneManager>();

		if (loadingBar == null)
			Debug.LogError("LoadingProgressBar [" + gameObject.name + "] no tiene asignada una LoadingBar", this);
		if (loadingBar == null || manager == null)
			this.enabled = false;
	}

	private void Update()
	{
		if (loadingBar == null || manager == null)
			return;
		loadingBar.fillAmount = manager.AsyncLoadProgress;
	}
}
