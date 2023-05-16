using UnityEngine;

namespace GunsNGhosts.Ghosts
{
	/// <summary> Basic Ghost behaviour that tries to reach a target while avoiding other ghosts. </summary>
	public class ChasingBehaviour : GhostBehaviour
	{
		/// <summary> Target position that this Ghost tries to reach. </summary>
		[Space] [SerializeField] protected Transform target = null;
		/// <summary> Direction this Ghost needs to move to reach de target. </summary>
		protected Vector3 targetDirection = Vector3.up;

		/// <summary> Current speed and direction of this Ghost. </summary>
		protected Vector3 speedVector = Vector3.zero;
		/// <summary> Maximum speed this Ghost can move at. </summary>
		[Space] [SerializeField] protected float maxSpeed = 0.05f;
		/// <summary> Acceleration that adds to the speed per second. </summary>
		[SerializeField] protected float acceleration = 0.5f;

		/// <summary>  Layers of objects that this Ghost will try to avoid touching. </summary>
		[Space] [SerializeField] protected LayerMask obstacleDetectionMask = 1 << 10;
		/// Distance from the Ghost at which obstacles can be detected.
		[SerializeField] protected float obstacleMinDistance = 0.5f;
		[SerializeField] protected float obstacleMaxDistance = 1;


		// ------------------------------------------------------------------
		#region Movement

		protected virtual void FixedUpdate() => Move();


		/// <summary> Move this Ghost according to its behaviour. </summary>
		protected virtual void Move()
		{
			// Calculate direction towards the player.
			targetDirection = GetTargetDirection();

			// Calculate the direction to avoid obstacles.
			Vector3 avoidingDirection = Vector3.zero;
			Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, obstacleMaxDistance, obstacleDetectionMask);
			foreach (Collider2D obstacle in obstacles)
			{
				if (obstacle.gameObject == gameObject)
					continue;

				// Calculate the direction and distance form the obstacle.
				Vector3 dir = transform.position - obstacle.transform.position;
				dir.z = 0;
				float dis = dir.magnitude;
				// We want to avoid the obstacle with an instesity of 1 if it is at the same position.
				// And with an intesity of 0 if its at the detection radius.
				float intensity = Utilities.Math.Remap(dis, obstacleMinDistance, obstacleMaxDistance, 1, 0);
				intensity = Mathf.Clamp01(intensity);

				avoidingDirection += dir.normalized * intensity;
			}

			//Caluculate the direction to move based.
			Vector3 finalDirection = avoidingDirection.normalized + targetDirection.normalized;

			// Accelerate towards the final direction.
			speedVector += finalDirection.normalized * acceleration * Time.deltaTime;
			if (speedVector.sqrMagnitude > Mathf.Pow(maxSpeed, 2))		// Limit the speed to the max.
				speedVector = speedVector.normalized * maxSpeed;
			transform.position += speedVector;
		}

		/// <summary> Calculate the direction this Ghost needs to move to reach the target. </summary>
		protected virtual Vector3 GetTargetDirection()
		{
			if (target == null)
				return Vector3.zero;
			return (target.position - transform.position).normalized;
		}

		#endregion


		// ------------------------------------------------------------------
		#region Properties

		public override Transform Target
		{
			get => target;
			set => target = value;
		}

		public override Vector3 Direction => targetDirection;

		#endregion
	}
}
