using UnityEngine;

namespace GunsNGhosts.Ghosts
{
	using GhostComponent = GunsNGhosts.IRequire<Ghost>; 

	/// <summary> Represents a single Ghost. With references to its basic of its components. </summary>
	public class Ghost : MonoBehaviour
	{
		[Space][SerializeField] GhostBehaviour[] behaviours = { };
		[SerializeField] GhostHealth health = null;
		[SerializeField] Animator animator = null;


		// ------------------------------------------------------------------

		private void Reset()
		{
			health = GetComponent<GhostHealth>();
			animator = GetComponent<Animator>();
		}

		private void Awake()
		{
			// Find all components that need a reference to a Ghost class in this GameObject and asign them this Ghost.
			GhostComponent[] components = GetComponentsInChildren<GhostComponent>();
			foreach (GhostComponent component in components)
				component.SetRequirement(this);
		}


		// ------------------------------------------------------------------
		#region Properties

		/// <summary> Reference to the fist behaviour of this Ghost. </summary>
		public GhostBehaviour Behaviour
		{
			get
			{
				if (behaviours.Length > 0)
					return behaviours[0];
				return null;
			}
		}

		/// <summary> Array of behaviours of this Ghost. </summary>
		public GhostBehaviour[] Behaviours => behaviours;

		/// <summary> Health component of this Ghost. </summary>
		public GhostHealth Health => health;

		/// <summary> The animator of this Ghost. </summary>
		public Animator Animator => animator;

		#endregion
	}
}
