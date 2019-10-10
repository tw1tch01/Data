using Microsoft.EntityFrameworkCore;

namespace DevGate.Data.Tests.TestClasses
{
	public class TestDbSet : DbSet<TestEntity>
	{
		public virtual DbSet<TestEntity> TestEntities { get; set; }
	}

	public class TestCreatedDbSet : DbSet<TestCreatedEntity>
	{
		public virtual DbSet<TestCreatedEntity> TestEntities { get; set; }

	}
}
