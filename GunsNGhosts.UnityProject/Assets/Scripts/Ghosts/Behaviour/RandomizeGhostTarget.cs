using UnityEngine;
using Pathfinding;

namespace GunsNGhosts.Ghosts
{
	/// <summary> Sets the target of a Ghost to a random point of the pathfinding. </summary>
	public class RandomizeGhostTarget : MonoBehaviour, IRequire<Ghost>
	{
		Transform playerTransform = null;
		Transform point;
		GridNode[] nodes = { };


		// ------------------------------------------------------------------

		private void Awake()
		{
			Debug.LogError("WARNINNG: this script was not working correctly.");
		}

		public void SetRequirement(Ghost requirement)
		{
			ghost = requirement;
			playerTransform = ReferenceProvider.GetReference<Player>().Transform;

			if (point == null)
			{
				point = new GameObject(transform.name + ".Target").transform;
				point.parent = transform.parent;
			}
			ghost.Behaviour.Target = point;
		}

		private void OnEnable()
		{
			AstarPath astar = AstarPath.active;
			nodes = astar.data.gridGraph.nodes;

			FindNewTarget();
		}


		// ------------------------------------------------------------------

		private void Update()
		{
			if (point == null)
				return;

			Debug.DrawLine(transform.position, ghost.Behaviour.Target.position, Color.red);

			Vector3 dir = transform.position - point.position;
			if (dir.sqrMagnitude <= Mathf.Pow(1, 2))
			{
				FindNewTarget();
			}
		}

		/// <summary> Set a new random target for the Ghost. </summary>
		public void FindNewTarget()
		{
			if (nodes.Length >= 0)
			{
				int i = Random.Range(0, nodes.Length - 1);
				point.position = (Vector3)nodes[i].position;
			}
			else
			{
				point.position = playerTransform.position;
			}
			ghost.Behaviour.Target = point;
		}


		// ------------------------------------------------------------------

		public Ghost ghost
		{
			get;
			set;
		}
	}
}
