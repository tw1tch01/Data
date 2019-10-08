namespace DevGate.Data.Entities
{
	/// <summary>
	/// Represents the abstract class that all entities should inherit from
	/// </summary>
	public abstract class BaseEntity
	{
		#region Fields

		internal virtual bool IsDeletable => true;

		#endregion Fields
	}
}