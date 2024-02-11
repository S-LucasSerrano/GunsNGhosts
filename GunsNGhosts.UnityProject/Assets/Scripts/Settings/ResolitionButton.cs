using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolitionButton : MonoBehaviour
{
	public Button button = null;
	public Vector2Int resolution = new(1024, 576);


	private void Reset()
	{
		button = GetComponent<Button>();
	}

	private void Start()
	{
		button.onClick.AddListener(() =>
		{
			Screen.SetResolution(resolution.x, resolution.y, FullScreenMode.Windowed);
		});
	}
}
