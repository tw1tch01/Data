using System.Collections.Generic;
using DevGate.Data.Entities;
using DevGate.Data.Entities.Audits;

namespace DevGate.Data.Other
{
	public class AuditFields
	{
		private readonly IReadOnlyCollection<string> _createdFields;
		private readonly IReadOnlyCollection<string> _deletedFields;
		private readonly IReadOnlyCollection<string> _updatedFields;

		public AuditFields()
		{
			_createdFields = new List<string> { nameof(ICreated.CreatedBy), nameof(ICreated.CreatedOn) };
			_deletedFields = new List<string> { nameof(NonDeletableEntity.DeletedBy), nameof(NonDeletableEntity.DeletedOn) };
			_updatedFields = new List<string> { nameof(IUpdated.UpdatedBy), nameof(IUpdated.UpdatedOn) };
		}

		public IDictionary<string, IReadOnlyCollection<string>> Fields => new Dictionary<string, IReadOnlyCollection<string>>
		{
			{ nameof(ICreated.Create), _createdFields },
			{ nameof(NonDeletableEntity.Delete), _deletedFields },
			{ nameof(IUpdated.Update), _updatedFields }
		};
	}
}