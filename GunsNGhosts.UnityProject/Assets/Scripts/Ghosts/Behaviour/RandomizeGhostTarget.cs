using UnityEngine;
using Pathfinding;

namespace GunsNGhosts.Ghosts
{
	/// <summary> Sets the target of a Ghost to a random point of the pathfinding. </summary>
	public class RandomizeGhostTarget : MonoBehaviour, IRequire<Ghost>
	{
		Transform point;
		GridNode[] nodes = { };


		// ------------------------------------------------------------------

		private void Start()
		{
			AstarPath astar = AstarPath.active;
			nodes = astar.data.gridGraph.nodes;

			FindNewTarget();
		}

		public void SetRequirement(Ghost requirement)
		{
			if (point == null)
				point = new GameObject(transform.name + ".Target").transform;

			ghost = requirement;
			ghost.Behaviour.Target = point;
		}


		// ------------------------------------------------------------------

		private void Update()
		{
			if (point == null)
				return;

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
				point.position = Game.Player.Transform.position;
			}
		}


		// ------------------------------------------------------------------

		public Ghost ghost
		{
			get;
			set;
		}
	}
}
