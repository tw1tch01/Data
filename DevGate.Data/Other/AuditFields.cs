using System.Collections.Generic;
using DevGate.Domain.Entities;
using DevGate.Domain.Entities.Audits;

namespace DevGate.Data.Other
{
	/// <summary>
	/// Retrieve all audit types and their respective fields
	/// </summary>
	public class AuditFields
	{
		private readonly IReadOnlyCollection<string> _createdFields;
		private readonly IReadOnlyCollection<string> _deletedFields;
		private readonly IReadOnlyCollection<string> _modifiedFields;

		/// <summary>
		/// Initializes new instance of <see cref="AuditFields"/>
		/// </summary>
		public AuditFields()
		{
			_createdFields = new List<string> { nameof(ICreatedAudit.CreatedBy), nameof(ICreatedAudit.CreatedOn) };
			_deletedFields = new List<string> { nameof(NonDeletableEntity.DeletedBy), nameof(NonDeletableEntity.DeletedOn) };
			_modifiedFields = new List<string> { nameof(IModifiedAudit.ModifiedBy), nameof(IModifiedAudit.ModifiedOn) };
		}

		/// <summary>
		/// Dictionary of audit types and their respective fields
		/// </summary>
		public IDictionary<string, IReadOnlyCollection<string>> Fields => new Dictionary<string, IReadOnlyCollection<string>>
		{
			{ nameof(ICreatedAudit), _createdFields },
			{ nameof(NonDeletableEntity), _deletedFields },
			{ nameof(IModifiedAudit), _modifiedFields }
		};
	}
}