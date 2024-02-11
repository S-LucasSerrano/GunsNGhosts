using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
	public List<Panel> panels = new();


	private void Start()
	{
		ShowPanel(panels[0].id);
	}

	public void ShowPanel(string id)
	{
		foreach(Panel panel in panels)
		{
			panel.gameObject.SetActive(panel.id == id);
		}
	}


	[System.Serializable]
    public class Panel
	{
		public string id;
		public GameObject gameObject;
	}
}
