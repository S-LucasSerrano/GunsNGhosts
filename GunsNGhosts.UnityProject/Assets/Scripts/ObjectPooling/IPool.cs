
namespace LucasSerrano.Pooling
{
	/// <summary> Base interface for object pools. </summary>
	public interface IPool <T>
	{
		/// <summary> Initialize this pool. </summary>
		public void Initialize();

		/// <summary> Get the next element of the pool. </summary>
		public T Get();
	}
}
