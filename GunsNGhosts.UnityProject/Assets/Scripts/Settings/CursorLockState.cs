using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLockState : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Unlock();
        }
        else if (Cursor.lockState != CursorLockMode.Confined
            && Input.GetKeyDown(KeyCode.Mouse0))
		{
            Lock();
		}
    }

    public void Lock()
	{
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Unlock()
	{
        Cursor.lockState = CursorLockMode.None;
    }
}
