using UnityEngine;
using Pathfinding;

namespace GunsNGhosts.Ghosts
{
	/// <summary> Ghost that tries to reach its target using the A* pathfinding project. </summary>
	[RequireComponent(typeof(Seeker))]
	public class PathfinderBehaviour : ChasingBehaviour
	{
		/// <summary> A* pathfinding component we use to calculate the path to the target. </summary>
		Seeker seeker = null;
		/// <summary> Path that this Ghost is currently following. </summary>
		Path path = null;
		/// <summary> Point in the path that this Ghost is trying to reach. </summary>
		int pathPoint = 0;


		// ------------------------------------------------------------------

		protected override void Start()
		{
			base.Start();
			seeker = GetComponent<Seeker>();
		}


		// ------------------------------------------------------------------

		protected virtual void Update()
		{
			if (target == null)
				return;

			// Recalculate the path to the player every X frames
			if (Time.frameCount % 3 == 0)
				seeker.StartPath(transform.position, target.position, SetPath);
			// If we are close enough to the target point, go to the next point in the path.
			if (targetDirection.sqrMagnitude < Mathf.Pow(0.1f, 2) && pathPoint < path.vectorPath.Count-1)
				pathPoint++;
		}

		/// <summary> Give this Ghost a new path to follow. </summary>
		/// This function is called when the Seeker component has ended calcualting the path to the target.
		void SetPath(Path newPath)
		{
			if (newPath.error)
				return;

			path = newPath;
			pathPoint = 1;
		}


		// ------------------------------------------------------------------

		/// The target direction for this Ghots is the direction towards the next point on the path.
		protected override Vector3 GetTargetDirection()
		{
			if (path == null || path.error)
				return base.GetTargetDirection();

			Vector3 targetDirection = (path.vectorPath[pathPoint] - transform.position);
			targetDirection.z = 0;
			return targetDirection;
		}


	}
}

