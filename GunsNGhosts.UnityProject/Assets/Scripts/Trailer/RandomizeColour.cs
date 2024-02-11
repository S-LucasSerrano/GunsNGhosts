using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeColour : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> sprietes = new();
    [SerializeField] Color[] colors = { };


	private void Reset()
	{
        SpriteRenderer s = GetComponent<SpriteRenderer>();	
        if (s != null)
		{
            sprietes.Add(s);
		}
	}

	private void Start()
	{
		int randomColor = Random.Range( 0, colors.Length );
		foreach(SpriteRenderer s in sprietes)
		{
			s.color = colors[randomColor];
		}
	}
}
