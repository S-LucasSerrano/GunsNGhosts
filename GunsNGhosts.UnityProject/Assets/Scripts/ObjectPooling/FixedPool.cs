
namespace LucasSerrano.Pooling
{
	/// <summary> Object pool with a fixed number of elements that returns them in a cycle. </summary>
	/// <typeparam name="T"> Type of the elements in the pool. </typeparam>
	public class FixedPool<T> : GenericPool<T>
	{
		// -----------------------------------------------------------------
		public FixedPool(CreateElementDelegate createElementAction = null, int size = 0)
		{
			this.createElementAction = createElementAction;
			this.size = size;
		}


		// -----------------------------------------------------------------
		public override T Get()
		{
			if (elements.Count <= 0)
				throw new System.Exception("You are trying to get an element of an empty pool. You may need to call Initialize() and/or assign a size bigger than 0.");

			T element = elements.Dequeue();
			elements.Enqueue(element);
			return element;
		}
	}
}
