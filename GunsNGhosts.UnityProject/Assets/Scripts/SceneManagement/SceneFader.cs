using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Componente que anima un elemento de la UI cambiando su trasparencia para hacer fade in y fade out. </summary>
[AddComponentMenu("Scene Package/Scene Fader")]
public class SceneFader : MonoBehaviour
{
	/// <summary> Imagen de la que vamos a cambiar el alpha para hacer fade. </summary>
	[SerializeField] Image image = null;
	/// <summary>  Tiempo que dura el fade. </summary>
	[SerializeField][Min(0)] float fadingTime = 0;

	/// <summary> Si estamos o no haciendo fade ahora mismo. </summary>
	bool fading = false;
	/// <summary> Referencia a la corrutina que esta haciendo fade. </summary>	
	Coroutine fadingRoutine = null;


	// ---------------------------------------------

	private void Reset() => image = GetComponent<Image>();


	// ---------------------------------------------
	#region Fade

	/// <summary> Vuelve la imagen opaca (true), o transparente (false). </summary>
	public void Fade(bool state)
	{
		// Si el GameObject esta desactivado, poner directamente en el estado final.
		if (gameObject.activeInHierarchy == false)		/// Las corrutinas no pueden activarse con el GameObject inactivo.
		{
			FadeInmidiatly(state);
			return;
		}

		SetActive(true);

		if (fadingRoutine != null) StopCoroutine(fadingRoutine);
		fadingRoutine = StartCoroutine(FadingRoutine(state));		
	}

	IEnumerator FadingRoutine(bool state)
	{
		float start	= state == true ? 0 : 1;    /* Empieza transparente y termina opaco		*/
		float end	= state == true ? 1 : 0;    /* o empieza opaco y termina transparente.	*/

		float timeCounter = 0;

		fading = true;		
		// Ir cambiando la transparencia de la imagen
		while (timeCounter < fadingTime)
		{
			/// Esta operacion devuelve 0 cuando contador == 0 y 1 cuando contador == fadingTime, en una progesion lineal.
			float progress = timeCounter / fadingTime;
			/// Usamos la funcion lerp para interpolar entre el valor inicial y el final.
			float targetAlpha = Mathf.Lerp(start, end, progress);
			SetAlpha(targetAlpha);

			timeCounter += Time.deltaTime;
			yield return null;
		}

		SetAlpha(end);
		SetActive(state);
		fading = false;
	}

	/// <summary> Vuelve la imagen opaca (true) o transparente (false) directamente, sin animacion. </summary>
	public void FadeInmidiatly(bool state)
	{
		float targetAplha = state ? 1 : 0;
		SetAlpha(targetAplha);
		SetActive(state);
	}

	/// <summary> Activa o desactiva la imagen que hace fade. </summary>
	void SetActive(bool value) => image.enabled = value;

	/// <summary> Cambia el alpha de la imagen que hace fade. </summary>
	void SetAlpha(float alpha)
	{
		Color color = image.color;
		color.a = alpha;
		image.color = color;
	}

	#endregion


	// ---------------------------------------------
	#region Properties

	/// <summary> Duración en segundos del fade. </summary>
	public float FadingTime
	{
		get { return fadingTime; }
		set
		{
			if (value < 0) value = 0;
			fadingTime = value;
		}
	}

	/// <summary> Si se esta o no haciendo fade ahora mismo. </summary>
	public bool Fading => fading;

	#endregion
}
