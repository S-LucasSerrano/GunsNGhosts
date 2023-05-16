using UnityEngine;

/// <summary>
/// Info used by the AddShakeByDistance() function of the CameraShaker. </summary>
[System.Serializable]
public struct ShakeByDistanceData
{
		[Tooltip ("Min distance to have in count.")]
	public float minDistance;	
		[Tooltip ("Max distance to have in count.")]
	public float maxDistance;

		[Tooltip ("Shake magnitude added at Min Distance or less.")]
	[Range(0, 1)] public float minMagnitude;
		[Tooltip ("Shake magnitude added at Max Distance or more.")]
	[Range(0, 1)] public float maxMagnitude;

		[Tooltip ("Curve that defines how the shake is reduced by the distance.\n" +
		"The X axis is distance. 0 = minDis and 1 = maxDis.\n" +
		"The Y axis is magnitude. 0 = minShake and 1 = maxShake.")]
	public AnimationCurve falloffCurve;


	public ShakeByDistanceData(float minDistance, float maxDistance, float minMagnitude, float maxMagnitude, AnimationCurve falloffCurve)
	{
		this.minDistance = minDistance;
		this.maxDistance = maxDistance;
		this.minMagnitude = minMagnitude;
		this.maxMagnitude = maxMagnitude;
		this.falloffCurve = falloffCurve;
	}

	public ShakeByDistanceData (float minDistance, float maxDistance, float minMagnitude, float maxMagnitude)
	{
		this.minDistance = minDistance;
		this.maxDistance = maxDistance;
		this.minMagnitude = minMagnitude;
		this.maxMagnitude = maxMagnitude;
		this.falloffCurve = AnimationCurve.Linear(0,1,1,0);
	}

	public ShakeByDistanceData(float minDistance, float maxDistance)
	{
		this.minDistance = minDistance;
		this.maxDistance = maxDistance;
		this.minMagnitude = 0;
		this.maxMagnitude = 1;
		this.falloffCurve = AnimationCurve.Linear(0,1,1,0);
	}
}
