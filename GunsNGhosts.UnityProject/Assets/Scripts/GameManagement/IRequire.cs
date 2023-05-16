
namespace GunsNGhosts
{
	/// <summary> Interface for objects that need to be provided with some data. </summary>
	public interface IRequire<T>
	{
		/// <summary> Set the requirement of this object. </summary>
		public void SetRequirement(T requirement);
	}
}
