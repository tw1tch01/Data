using DevGate.Data.Contexts;
using DevGate.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevGate.Data.Tests.TestClasses
{
	public class TestDbContext : DbContext, IDbContext
	{
		public TestDbContext()
		{
		}

		public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
		{
		}

		public virtual DbSet<TestEntity> TestEntities { get; set; }
		public virtual DbSet<TestCreatedEntity> TestCreatedEntities { get; set; }

		public virtual DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
		{
			return base.Set<TEntity>();
		}
	}
}