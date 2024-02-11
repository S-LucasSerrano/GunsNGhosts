using LucasSerrano.Pooling;
using UnityEngine;

namespace GunsNGhosts
{
	/// <summary> Component for GameObjects that are part of a dynamic pool. <para></para>
	/// It returns the object to its pool when on disable. </summary>
	public class PooleableGameObject : MonoBehaviour, IPooleable
	{
		public DynamicPool<GameObject> Pool { get; set; }


		private void OnDisable()
		{
			if (Pool == null)
				return;

			Pool.Release(this.gameObject);
		}
	}
}
