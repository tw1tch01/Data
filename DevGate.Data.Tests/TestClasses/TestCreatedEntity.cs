using System;
using DevGate.Domain.Entities;
using DevGate.Domain.Entities.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevGate.Data.Tests.TestClasses
{
	public class TestCreatedEntity : BaseEntity, ICreatedAudit
	{
		public string CreatedBy { get; set; }
		public DateTime CreatedOn { get; set; }
		public int Id { get; set; }
	}

	public class TestCreatedEntityConfiguration : IEntityTypeConfiguration<TestCreatedEntity>
	{
		public void Configure(EntityTypeBuilder<TestCreatedEntity> builder)
		{
			builder.HasKey(nameof(TestCreatedEntity.Id));
			builder.Property(nameof(TestCreatedEntity.CreatedOn)).IsRequired();
			builder.Property(nameof(TestCreatedEntity.CreatedBy)).IsRequired();
		}
	}
}