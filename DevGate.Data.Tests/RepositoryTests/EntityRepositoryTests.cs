using DevGate.Data.Contexts;
using DevGate.Data.Entities;
using DevGate.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DevGate.Data.Tests.RepositoryTests
{
	[TestFixture]
	public class EntityRepositoryTests
	{
		[Test]
		public void Remove_CallsDbContextRemove()
		{
			// Arrange
			var mockDbSet = new Mock<DbSet<BaseEntity>>();
			var mockDbContext = new Mock<TestDbContext>();
			var mockEntity = new Mock<BaseEntity>();

			var repo = CreateRepo(mockDbContext);

			mockDbContext.Setup(db => db.Set<BaseEntity>()).Returns(mockDbSet.Object);

			// Act
			repo.Remove(mockEntity.Object);

			// Assert
			mockDbSet.Verify(set => set.Remove(mockEntity.Object), Times.Once, "Remove method should be called.");
		}

		private EntityRepository<TContext> CreateRepo<TContext>(Mock<TContext> mockContext = null, Mock<ILogger<TContext>> mockLogger = null) where TContext : DbContext, IDbContext
		{
			mockContext ??= new Mock<TContext>();
			mockLogger ??= new Mock<ILogger<TContext>>();

			return new EntityRepository<TContext>(mockContext.Object, mockLogger.Object);
		}

		public class TestDbContext : DbContext, IDbContext
		{
			public virtual DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
			{
				return base.Set<TEntity>();
			}
		}
	}
}