using System;

namespace DevGate.Data.Entities.Audits
{
	/// <summary>
	/// Represents required fields for a creatable-audit entity
	/// </summary>
	public interface ICreated
	{
		/// <summary>
		/// Who created the entity
		/// </summary>
		string CreatedBy { get; }

		/// <summary>
		/// When the entity was created
		/// </summary>
		DateTime CreatedOn { get; }

		/// <summary>
		/// Set the audit fields after the entity is created
		/// </summary>
		/// <param name="createdBy"><see cref="ICreated.CreatedBy"/></param>
		/// <param name="createdOn"><see cref="ICreated.CreatedOn"/></param>
		void Create(string createdBy, DateTime createdOn);
	}
}