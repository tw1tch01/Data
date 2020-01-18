using System;

namespace Data.IntegrationTests
{
    internal class AuditedEntity
    {
        public Guid Id { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedProcess { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedProcess { get; set; }
        public string Name { get; set; }
    }
}