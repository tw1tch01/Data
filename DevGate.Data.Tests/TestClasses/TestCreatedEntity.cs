using System;
using DevGate.Data.Entities;
using DevGate.Data.Entities.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevGate.Data.Tests.TestClasses
{
	public class TestCreatedEntity : BaseEntity, ICreated
	{
		public int Id { get; set; }

		public string CreatedBy { get; private set; }

		public DateTime CreatedOn { get; private set; }

		public virtual void Create(string createdBy, DateTime createdOn)
		{
			CreatedBy = createdBy;
			CreatedOn = createdOn;
		}
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