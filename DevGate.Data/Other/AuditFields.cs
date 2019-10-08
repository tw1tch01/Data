using System.Collections.Generic;
using DevGate.Data.Entities;
using DevGate.Data.Entities.Audits;

namespace DevGate.Data.Other
{
	/// <summary>
	/// Retrieve all audit types and their respective fields
	/// </summary>
	public class AuditFields
	{
		private readonly IReadOnlyCollection<string> _createdFields;
		private readonly IReadOnlyCollection<string> _deletedFields;
		private readonly IReadOnlyCollection<string> _updatedFields;

		/// <summary>
		/// Initializes new instance of <see cref="AuditFields"/>
		/// </summary>
		public AuditFields()
		{
			_createdFields = new List<string> { nameof(ICreated.CreatedBy), nameof(ICreated.CreatedOn) };
			_deletedFields = new List<string> { nameof(NonDeletableEntity.DeletedBy), nameof(NonDeletableEntity.DeletedOn) };
			_updatedFields = new List<string> { nameof(IUpdated.UpdatedBy), nameof(IUpdated.UpdatedOn) };
		}

		/// <summary>
		/// Dictionary of audit types and their respective fields
		/// </summary>
		public IDictionary<string, IReadOnlyCollection<string>> Fields => new Dictionary<string, IReadOnlyCollection<string>>
		{
			{ nameof(ICreated.Create), _createdFields },
			{ nameof(NonDeletableEntity.Delete), _deletedFields },
			{ nameof(IUpdated.Update), _updatedFields }
		};
	}
}