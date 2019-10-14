using DevGate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevGate.Data.Tests.TestClasses
{
	public class TestEntity : BaseEntity
	{
		public int Id { get; set; }
	}

	public class TestEntityConfiguration : IEntityTypeConfiguration<TestEntity>
	{
		public void Configure(EntityTypeBuilder<TestEntity> builder)
		{
			builder.HasKey(nameof(TestEntity.Id));
		}
	}
}