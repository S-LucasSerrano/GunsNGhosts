
namespace LucasSerrano.Pooling
{
	/// <summary> Base interface for object pools. </summary>
	public interface IPool <T>
	{
		/// <summary> Get the next element of the pool. </summary>
		public T Get();
	}
}
