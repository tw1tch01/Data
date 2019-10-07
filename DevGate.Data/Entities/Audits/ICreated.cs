using System;

namespace DevGate.Data.Entities.Audits
{
	public interface ICreated
	{
		string CreatedBy { get; }
		DateTime CreatedOn { get; }

		void Create(string createdBy, DateTime createdOn);
	}
}