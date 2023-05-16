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
					return transform.forward;
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

			transform.LookAt(transform.position + TargetDirection, Vector3.up);
		}
	}
}
