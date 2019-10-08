using System;

namespace DevGate.Data.Entities
{
	/// <summary>
	/// Represents a non-deletable entity
	/// </summary>
	public abstract class NonDeletableEntity : BaseEntity
	{
		#region Properties

		/// <summary>
		/// Who "deleted" the entity
		/// </summary>
		public string DeletedBy { get; private set; }

		/// <summary>
		/// When the entity was "deleteD"
		/// </summary>
		/// <remarks>If value is null, entity has NOT been deleted</remarks>
		public DateTime? DeletedOn { get; private set; }

		/// <summary>
		/// Indication of whether the entity has been "soft" deleted
		/// </summary>
		public bool IsDeleted => DeletedOn.HasValue;

		/// <summary>
		/// Ensures the entity cannot be deleted
		/// </summary>
		internal override bool IsDeletable => false;

		#endregion Properties

		/// <summary>
		/// Sets the audit fields, after the entity is soft "deleted"
		/// </summary>
		/// <param name="deletedBy"></param>
		/// <param name="deletedOn"></param>
		public virtual void Delete(string deletedBy, DateTime deletedOn)
		{
			DeletedBy = deletedBy;
			DeletedOn = deletedOn;
		}

		/// <summary>
		/// Unsets the audit fields to restore an entity
		/// </summary>
		public virtual void Restore()
		{
			DeletedBy = null;
			DeletedOn = null;
		}
	}
}