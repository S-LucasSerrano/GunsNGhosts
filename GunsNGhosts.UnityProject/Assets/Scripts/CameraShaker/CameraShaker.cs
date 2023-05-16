using UnityEngine;
using System.Collections;

/// <summary> Componenet that shakes the camera. </summary>
[AddComponentMenu("Miscellaneous/Camera Shaker")] 
public class CameraShaker : MonoBehaviour
{
	#region Variables

	/// <summary> Current amount of shake. </summary>
	/// At 0 there is no shake. At 1 the shake is at its max values.
	protected float trauma = 0;
	/// <summary> Current amount of directional shake. </summary>
	protected float directionalTrauma = 0;

	/// <summary> Rotation of the camera when not shaking. </summary>
	protected Quaternion originalRot = new Quaternion();
	/// <summary> Local position of the camera when not shaking. </summary>
	protected Vector3 originalPos = new Vector3();

	[Tooltip("Does the camera shake in 3D or 2D?")]
	[Space][SerializeField] ShakeTypes shakeType = ShakeTypes._3D;

		[Tooltip("Displacement in the X axis when the Trauma is at 1.")][Space]
	[SerializeField] float xMaxShake = 7f; 
		[Tooltip("Displacement in the Y axis when the Trauma is at 1.")]
	[SerializeField] float yMaxShake = 10f;
		[Tooltip("Displacement in the Z axis when the Trauma is at 1.")]
	[SerializeField] float zMaxShake = 3f;


		[Tooltip("Time in seconds that takes the Trauma to drop from 1 to 0.")]
	[Space][SerializeField] float traumaReductionTime = .8f;

		[Tooltip("Curve that defines how the Trauma affects the shake, determining how it reduces over time.")]
	[SerializeField] AnimationCurve shakeScalingCurve = DefaultScalingCurve();


		[Tooltip("Scale of the perlin noise map used to generate pseudo-random shake. \n" +
		"Increasing this value increases the randomness of the shakes.")]
	[Space][SerializeField] float noiseScale = 17;

	/// <summary> Coordinate from the noise map that determine the direction the camera shakes in each axis. </summary>
	protected float noiseCoordinate = 0;


	/// <summary> True if the object is shaking. </summary>
	protected bool shaking = false;

	#endregion


	// -----------------------------------------------------
	#region Start

	private void Start()
	{
		// Save the original rotation and position to restore them after the shake.
		originalRot = transform.localRotation;
		originalPos = transform.localPosition;
	}

	#endregion


	// -----------------------------------------------------
	#region AddShake()

	/// <summary> Increase the amount of camera shake. </summary>
	/// <param name="magnitude"> The amount of shake to be added. Must be between 0 and 1. </param>
	public void AddShake(float magnitude)
	{
		// Add trauma and clamp it between 0 and 1.
		trauma += magnitude;
		trauma = Mathf.Clamp01(trauma);

		// Start shaking if we're not already doing it.
		if (!shaking) StartCoroutine(ShakingRoutine());
	}

	#endregion


	#region AddLimitedShake()

	/// <summary> Increase the cam shake by [magnitude], but never more than [max]. </summary>
	/// <param name="magnitude"> The amount of shake to be added. Must be between 0 and 1. </param>
	/// <param name="max"> The shake can only add to be as high as this value. </param>
	public void AddLimitedShake(float magnitude, float max)
	{
		if (trauma >= max)
			return;
		if (max - trauma < magnitude)
			AddShake(max - magnitude);
		else
			AddShake(magnitude);
	}

	/// <summary> Set the camera shake to [magnitude], only if it is not already more than that. </summary>
	/// <param name="magnitude">The amount of shake to be added. Must be between 0 and 1.</param>
	public void AddLimitedShake(float magnitude)
	{
		if (trauma >= magnitude)
			return;
		AddShake(magnitude - trauma);
	}

	#endregion


	#region SetShake()

	/// <summary> Sets camera shake to a specific value. </summary>
	/// <param name="magnitude"> The value for the shake to be set to. Must be between 0 and 1. </param>
	public void SetShake(float magnitude)
	{
		magnitude = Mathf.Clamp01(magnitude);
		trauma = 0;
		AddShake(magnitude);
	}

	#endregion


	#region AddShakeByDistance()

	/// <summary> Shake the camera based on the distance to a point.
	///<list type="bullet">
	///		Using a curve to define the amount of shake over distance. <item/>
	/// </list></summary>
	public void AddShakeByDistance(Vector3 origin, float minDistance, float maxDistance, float minMagnitude, float maxMagnitude, AnimationCurve distanceToShakeMappingCurve)
	{
		float value = (transform.position - origin).magnitude;		// Calculate the distance from the camera to the origin.
		value = Mathf.Clamp(value, minDistance, maxDistance);		// Clamp that distance between min and max.
		value = Remap(value, minDistance, maxDistance, 0, 1);		// Remap that distance to be between 0 and 1.
		value = distanceToShakeMappingCurve.Evaluate(value);		// Evaluate the curve at that point.
		value = Remap(value, 0, 1,  minMagnitude, maxMagnitude);	// Remap again between min and max shake to get the magnitude to add.

		AddLimitedShake(value);
	}

	/// <summary> Shake the camera based on the distance to a point.
	/// <list type="bullet">
	///		Reducing the shake linearly with the distance. <item/>
	/// </list></summary>
	public void AddShakeByDistance(Vector3 origin, float minDistance, float maxDistance, float minMagnitude, float maxMagnitude)
	{
		AddShakeByDistance(origin, minDistance, maxDistance, minMagnitude, maxMagnitude, AnimationCurve.Linear(0,1,1,0));
	}

	/// <summary> Shake the camera based on the distance to a point.
	/// <list type="bullet">
	///		Adding 0 magnitude at [maxDistance] and 1 at [minDistance] <item/>
	///		Using a curve to define the amount of shake over distance. <item/>
	/// </list></summary>
	public void AddShakeByDistance(Vector3 origin, float minDistance, float maxDistance, AnimationCurve distanceToShakeMappingCurve)
	{
		AddShakeByDistance(origin, minDistance, maxDistance, 0, 1, distanceToShakeMappingCurve);
	}

	/// <summary> Shake the camera based on the distance to a point.
	/// <list type="bullet">
	///		Adding 0 magnitude at [maxDistance] and 1 at [minDistance] <item/>
	///		Reducing the shake linearly with the distance. <item/>
	/// </list></summary>
	public void AddShakeByDistance(Vector3 origin, float minDistance, float maxDistance)
	{
		AddShakeByDistance(origin, minDistance, maxDistance, 0, 1);
	}

	/// <summary> Shake the camera based on the distance to a point.
	/// <list type="bullet">
	///		Using a data struct that defines how to calculate the magnitude to add.
	/// </list></summary>
	public void AddShakeByDistance(Vector3 origin, ShakeByDistanceData data)
	{
		AddShakeByDistance(origin, data.minDistance, data.maxDistance, data.minMagnitude, data.maxMagnitude, data.falloffCurve);
	}

	#endregion


	// ---------
	#region DirectionalShake()

	/// <summary> Currently active DirectionalShake coroutine. </summary>
	protected Coroutine _directionalShakeRoutine = null;

	/// <summary> Shakes the camera in a direction in relation to the screen.
	/// <para></para> The directional shake only works if the camera is not shaking. </summary>
	/// <param name="shake"> Distance and direction to shake the camera. </param>
	/// <param name="time"> Time that takes for the camera to return to its position. </param>
	public void DirectionalShake(Vector2 shake, float time)
	{
		if (shaking) return;

		if (_directionalShakeRoutine != null) StopCoroutine(_directionalShakeRoutine);
		_directionalShakeRoutine = StartCoroutine(DirectionalShakeRoutine( shake, time ));
	}

	/// <summary> Shakes the camera in a direction in relation to the screen.
	/// <para></para> The directional shake only works if the camera is not shaking. </summary>
	/// <param name="shake"> Distance and direction to shake the camera. </param>
	public void DirectionalShake(Vector2 shake)
	{
		if (shaking) return;

		// If not time is provided,
		// the time depends on how much the regular shake will take to reach 0 from that magnitude.
		float maxMagnitude = MaxShake.magnitude;
		float targetMagnitude = shake.magnitude;
		float targetTime = (targetMagnitude * traumaReductionTime) / maxMagnitude;

		DirectionalShake(shake, targetTime);
	}

	/// <summary> Coroutine that shakes the camera in a direction, slowly returning to its original position. </summary>
	protected virtual IEnumerator DirectionalShakeRoutine(Vector2 shake, float time)
	{
		Vector3 movementShake = new Vector3(shake.x, shake.y, 0);
		Vector3 rotationShake = new Vector3(-shake.y, shake.x, 0);
		float timeCounter = time;

		// Directional shake loop. If the camera starts shaking, the directional shake stops.
		while (shaking == false && timeCounter > 0)
		{
			float scaledShake = timeCounter / time;

			if (shakeType == ShakeTypes._3D)
			{
				transform.localRotation = originalRot;
				transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + rotationShake * scaledShake);
			}
			else
			{
				transform.localPosition = originalPos;
				transform.localPosition += movementShake * scaledShake;
			}

			timeCounter -= Time.deltaTime;
			yield return null;
		}

		transform.localPosition = originalPos;
		transform.localRotation = originalRot;
	}

	#endregion


	// -----------------------------------------------------
	#region Shaking Routine

	/// <summary> Coroutine that shakes the camera each frame, reducing the trauma until it's 0. </summary>
	protected virtual IEnumerator ShakingRoutine()
	{
		shaking = true;

		// Shaking loop.
		while (trauma > 0)
		{
			transform.localRotation = originalRot;
			transform.localPosition = originalPos;

			if (shakeType == ShakeTypes._3D)
			{
				RotationShake(Vector3.right);
				RotationShake(Vector3.up);
				RotationShake(Vector3.forward);
			}
			else
			{
				MovementShake(Vector3.right);
				MovementShake(Vector3.up);
				RotationShake(Vector3.forward);
			}

			// Move through the perlin noise.
			noiseCoordinate += Time.deltaTime;

			// Reduce Trauma.
			float remainingTime = traumaReductionTime * trauma;
			trauma = (remainingTime - Time.deltaTime) / traumaReductionTime;

			yield return null;
		}

		transform.localRotation = originalRot;
		transform.localPosition = originalPos;

		shaking = false;
	}

	/// <summary> Rotate this GameObject in a pseudo-random direction of the given axis. </summary>
	protected void RotationShake(Vector3 axis)
	{
		float maxShake = GetMaxShakeInAxis(axis);
		float scaledTrauma = shakeScalingCurve.Evaluate(trauma);
		float shake = maxShake * scaledTrauma;

		float randomDirection = GetDirectionForAxis(axis);

		transform.localRotation = Quaternion.Euler( transform.localRotation.eulerAngles + (axis * randomDirection) * shake );
	}

	/// <summary> Moves this GameObject in a pseudo-random direction of the given axis. </summary>
	protected void MovementShake(Vector3 axis)
	{
		float maxShake = GetMaxShakeInAxis(axis);
		float scaledTrauma = shakeScalingCurve.Evaluate(trauma);
		float shake = maxShake * scaledTrauma;

		float randomDirection = GetDirectionForAxis(axis);

		transform.localPosition += (axis * randomDirection) * shake;
	}

	/// <summary> Get the max displacement allowed for the given axis. <para></para>
	/// Expects Vector3.right, Vector3.up or Vector3.forward </summary>
	protected float GetMaxShakeInAxis(Vector3 axis)
	{
		if (axis == Vector3.right)
			return xMaxShake;
		if (axis == Vector3.up)
			return yMaxShake;
		if (axis == Vector3.forward)
			return zMaxShake;
		return 0;
	}

	/// <summary> Get a direction to shake in the given axis extracted from a perlin noise map. <para></para>
	/// Expects Vector3.right, Vector3.up or Vector3.forward </summary>
	protected float GetDirectionForAxis(Vector3 axis)
	{
		/* Each axis of the camera has assigned its own X coordinate of the 2D perlin noise map. But all share the same Y coordinate.
		 * Moving that Y coordinate each frame gives a direction to shake that feels random but related to the previous frame. */

		float xCoor = 0.5f;
		float yCoor = noiseCoordinate;

		if (axis == Vector3.right)
			xCoor = 0.1f;
		else if (axis == Vector3.up)
			xCoor = 0.5f;
		else if (axis == Vector3.forward)
			xCoor = 0.9f;

		xCoor = xCoor * noiseScale;
		yCoor = yCoor * noiseScale;
		float value = Mathf.PerlinNoise( xCoor, yCoor);

		return Remap(value, 0, 1, -1, 1);
	}

	#endregion


	// -----------------------------------------------------
	#region Utils

	/// <summary> Remap a value from one range to another. </summary>
	protected static float Remap(float value, float oldFrom, float oldTo, float newFrom, float newTo)
	{
		return (value - oldFrom) / (oldTo - oldFrom) * (newTo - newFrom) + (newFrom);
	}

	/// <summary> Returns a default easy-in curve to initialize shakeScalingCurve. </summary>
	protected static AnimationCurve DefaultScalingCurve()
	{
		AnimationCurve curve = new AnimationCurve();

		Keyframe key = new Keyframe(0, 0, 0, 0);
		curve.AddKey(key);

		key = new Keyframe(1, 1, 2, 2);
		curve.AddKey(key);

		return curve;
	}

	#endregion


	// -----------------------------------------------------
	#region Public Get & Set

	/// <summary> Current amount of shake. </summary>
	public float Trauma
	{
		get { return trauma; }
	}

	/// <summary> True if the object is shaking. </summary>
	public bool Shaking
	{
		get { return shaking; }
	}

	/// <summary> Does the camera shake in 3D or 2D? </summary>
	public ShakeTypes ShakeType
	{
		get { return shakeType; }
		set { shakeType = value; }
	}


	/// <summary> Max shake displacement. </summary>
	public Vector3 MaxShake
	{
		get	{ return new Vector3(xMaxShake, yMaxShake, zMaxShake); }

		set
		{
			xMaxShake = value.x;
			yMaxShake = value.y;
			zMaxShake = value.z;
		}
	}

	/// <summary> Max shake displacement in the X axis. </summary>
	public float XMaxShake
	{
		get { return xMaxShake; }
		set { xMaxShake = value; }
	}
	/// <summary> Max shake displacement in the Y axis. </summary>
	public float YMaxShake
	{
		get { return yMaxShake; }
		set { yMaxShake = value; }
	}
	/// <summary> Max shake displacement in the Z axis. </summary>
	public float ZMaxShake
	{
		get { return zMaxShake; }
		set { zMaxShake = value; }
	}


	/// <summary> Time in seconds that takes the Trauma to drop from 1 to 0. </summary>
	public float TraumaReductionTime
	{
		get { return traumaReductionTime; }
		set { traumaReductionTime = value; }
	}

	/// <summary> Curve that defines how the amount of Trauma affects the shake. </summary>
	public AnimationCurve ScalingCurve
	{
		get { return shakeScalingCurve; }
		set { shakeScalingCurve = value; }
	}


	/// <summary> Scale of the perlin noise map used to generate pseudo-random shake. <para/>
	///  Increasing this value increases the randomness of the camera shake. </summary>
	public float PerlinNoiseScale
	{
		get { return noiseScale; }
		set { noiseScale = value; }
	}

	#endregion


	// -----------------------------------------------------
	#region Definitions

	/// <summary> Types of shake displacement. </summary>
	public enum ShakeTypes
	{
		/// <summary> The camera rotates on every axis. </summary>
		[Tooltip("The camera rotates on every axis.")]
		_3D,
		
		/// <summary> The camera moves on the X and Y axis, and rotates on the Z. </summary>
		[Tooltip("The camera moves on the X and Y axis, and rotates on the Z.")]
		_2D
	}

	#endregion
}
