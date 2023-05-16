using UnityEngine;

namespace GunsNGhosts.Ghosts
{
	/// <summary> Basic class for Ghosts' behaviour scripts. </summary>
	public abstract class GhostBehaviour : MonoBehaviour, IRequire<Ghost>
	{
		public Ghost Manager { get; set; }
		public Ghost ghost { get; set; }

		/// <summary> Target of this Ghost behaviour. </summary>
		public abstract Transform Target { get; set; }

		/// <summary> Direction of this Ghost behaviour. </summary>
		public abstract Vector3 Direction { get; }

		public void SetRequirement(Ghost requirement) => ghost = requirement;
	}
}
