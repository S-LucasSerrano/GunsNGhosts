using System.Collections.Generic;

namespace LucasSerrano.Pooling
{
	/// <summary> Dynamic pool of objects that creates new objets when there are no objects to return. </summary>
	public class DynamicPool<T> : GenericPool<T>
	{
		/// <summary> List of elements from the pool that currelty in use, and unavialable to be returned. </summary>
		List<T> usedElements = new List<T>();


		// -----------------------------------------------------------------
		#region Constructor

		public DynamicPool(CreateElementDelegate createElementAction = null, int defaultSize = 0)
		{
			this.createElementAction = createElementAction;
			this.size = defaultSize;
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Pooling

		/// <summary> Get the next element of the pool. <para></para>
		/// Elements need to be returned to the pool using the Release() function before they can be used again. </summary>
		public override T Get()
		{
			T element;

			// If there are free elements in the pool, return the first one.
			// If not, create a new one.
			if ( elements.Count > 0 )
				element = elements.Dequeue();
			else
				element = createElementAction();
				
			// Add the element to the used list and return it.
			usedElements.Add(element);
			return element;
		}

		/// <summary> Return an element to the pool so it can be used again. </summary>
		public void Release(T element)
		{
			if (!usedElements.Contains(element))
				return;

			elements.Enqueue(element);
			usedElements.Remove(element);
		}

		#endregion
	}
}
