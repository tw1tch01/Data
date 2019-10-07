using System;

namespace DevGate.Data.Entities
{
	public abstract class NonDeletableEntity : BaseEntity
	{
		#region Properties

		public string DeletedBy { get; private set; }
		public DateTime? DeletedOn { get; private set; }
		public bool IsDeleted => DeletedOn.HasValue;
		internal override bool IsDeletable => false;

		#endregion Properties

		public virtual void Delete(string deletedBy, DateTime deletedOn)
		{
			DeletedBy = deletedBy;
			DeletedOn = deletedOn;
		}

		public virtual void Restore()
		{
			DeletedBy = null;
			DeletedOn = null;
		}
	}
}