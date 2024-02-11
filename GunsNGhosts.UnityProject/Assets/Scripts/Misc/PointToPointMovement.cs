using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Move an object between different points. </summary>
public class PointToPointMovement : MonoBehaviour
{
    /// <summary> The way this object is going to move from point to point. </summary>
    [Space] [SerializeField] MovementTypes movementType = MovementTypes.Loop;
    /// <summary> Bool that the ping pong movement type uses to define if the object is moving up or down the list. </summary>
    bool movingForward = true;

    /// <summary> Speed at which this object moves. </summary>
    [Space][SerializeField] float speed = 1;
    /// <summary> Time the object waits after reaching a point before it goes to the next one. </summary>
    [SerializeField] float timeBetweenPoints = 0;

    /// <summary> List of points this objects is going to move to. </summary>
    [Space][SerializeField] List<Transform> points = new();
    /// <summary> The index of the point in the list that this object is moving towards. </summary>
    int currentPointIndex = 0;
    /// <summary> Point that this object is moving towards. </summary>
    Transform currentPoint { get => points[currentPointIndex]; }
    /// <summary> Distance to the point at which we will consider the object has reached it. </summary>
    [SerializeField] float closeDistance = 0.1f;


	// ---------------------------------------------------------------

	void OnEnable()
	{
        StartCoroutine(MovingRoutine());
	}

    IEnumerator MovingRoutine()
	{
        while(true)
		{
            // Calculate the direction to the point.
            Vector3 dir = currentPoint.position - transform.position;
            dir.z = 0;
            // Move towards the point.
            transform.position += dir.normalized * speed * Time.deltaTime;

            // If the object is close enough to the point, start moving to the next.
            if (dir.sqrMagnitude <= closeDistance * closeDistance)
            {
                if (timeBetweenPoints > 0)
                    yield return new WaitForSeconds( timeBetweenPoints );

                if (movementType == MovementTypes.Loop)
                    SetNextLoopPoint();
                else
                    SetNextPingPongPoint();
            }

            yield return new WaitForFixedUpdate();
        }
	}

    /// <summary> Get the next point to cycle through the points in a loop. </summary>
    void SetNextLoopPoint()
	{
        if (currentPointIndex + 1 < points.Count)
            currentPointIndex++;
        else
            currentPointIndex = 0;
    }

    /// <summary> Get the next point to cycle through the points back and forward. </summary>
    void SetNextPingPongPoint()
	{
        if (movingForward)
		{
            if (currentPointIndex + 1 < points.Count)
                currentPointIndex++;
            else
			{
                movingForward = false;
                currentPointIndex--;

            }
        }
        else
		{
            if (currentPointIndex - 1 >= 0)
                currentPointIndex--;
            else
            {
                movingForward = true;
                currentPointIndex++;
            }
        }
	}


    // --------------------------------------------------------------
	#region Definitions

    public enum MovementTypes
	{
        Loop, 
        PingPong
	}

	#endregion
}
