using System;

namespace DevGate.Data.Entities.Audits
{
	/// <summary>
	/// Represents required fields for an updatable-audit entity
	/// </summary>
	public interface IUpdated
	{
		/// <summary>
		/// Who updated the entity
		/// </summary>
		string UpdatedBy { get; }

		/// <summary>
		/// When the entity was updated
		/// </summary>
		/// <remarks>If property is null, entity has NOT been updated</remarks>
		DateTime? UpdatedOn { get; }

		/// <summary>
		/// Set the audit fields after the entity is updated
		/// </summary>
		/// <param name="updatedBy"><see cref="IUpdated.UpdatedBy"/></param>
		/// <param name="updatedOn"><see cref="IUpdated.UpdatedOn"/></param>
		void Update(string updatedBy, DateTime updatedOn);
	}
}