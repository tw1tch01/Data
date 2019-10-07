using System;

namespace DevGate.Data.Entities.Audits
{
	public interface IUpdated
	{
		string UpdatedBy { get; }
		DateTime? UpdatedOn { get; }

		void Update(string updatedBy, DateTime updatedOn);
	}
}