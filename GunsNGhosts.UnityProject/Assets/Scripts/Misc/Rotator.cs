using UnityEngine;

/// <summary> Constatly rotates an object in the Z axis. </summary>
public class Rotator : MonoBehaviour
{
    [Space] [SerializeField] float speed = 100;

    void Update()
    {
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}
