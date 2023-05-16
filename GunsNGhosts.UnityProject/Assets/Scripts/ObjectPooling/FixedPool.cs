using System.Collections.Generic;

namespace LucasSerrano.Pooling
{
	/// <summary> Object pool with a fixed number of elements that returns them in a cycle. </summary>
	/// <typeparam name="T"> Type of the elements in the pool. </typeparam>
	public class FixedPool<T> : IPool<T>
	{
		/// <summary> Delegate defining a function to create new elements for the pool. </summary>
		public delegate T CreateElement();

		/// <summary> List of elements that will be returned one at a time. </summary>
		Queue<T> elements = new Queue<T>();


		// ----------------------------------------------------------------------

		/// <param name="createElementAction"> Function called when creating a new element for the pool. </param>
		/// <param name="poolCount"> Number of elements that will be created in the pool. </param>
		public FixedPool(CreateElement createElementAction, int poolCount)
		{
			for (int i = 0; i < poolCount; i++)
				elements.Enqueue(createElementAction());
		}


		// ----------------------------------------------------------------------

		public T Get()
		{
			T element = elements.Dequeue();
			elements.Enqueue(element);
			return element;
		}
	}
}
