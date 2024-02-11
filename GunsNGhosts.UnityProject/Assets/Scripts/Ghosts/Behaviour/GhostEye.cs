using UnityEngine;

namespace GunsNGhosts.Ghosts
{
	/// <summary> GameObject that looks in the direction of a Ghost behaviour. </summary>
	public class GhostEye : MonoBehaviour,  IRequire<Ghost>
	{
		/// <summary> Ghost that this component is part of. </summary>
		public Ghost ghost { get; set; }

		/// <summary> Direction of the Ghost behaviour. </summary>
		public Vector3 TargetDirection
		{
			get
			{
				if (ghost == null || ghost.Behaviour == null)
					return Vector3.right;
				Vector3 dir = ghost.Behaviour.Direction;
				dir.z = 0;
				return dir;
			}
		}


		// ------------------------------------------------------------------

		public void SetRequirement(Ghost requirement)
		{
			ghost = requirement;
		}


		// ------------------------------------------------------------------

		private void Update()
		{
			if (ghost == null)
				return;

			Vector3 lookinPos = transform.position + TargetDirection;

			// Keep the position we are going to look at in 2D.
			lookinPos.z = transform.position.z;
			// Make sure that the position we want to look at is never the same position we are at.
			if (lookinPos.x == transform.position.x && transform.position.y == transform.position.y)
				lookinPos = transform.position + Vector3.right;

			transform.LookAt(lookinPos, Vector3.up);
		}
	}
}
