using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevGate.Data.Contexts;
using DevGate.Data.Repositories;
using DevGate.Data.Tests.TestClasses;
using DevGate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DevGate.Data.Tests.RepositoryTests
{
	public partial class EntityRepositoryTests
	{
		[Test]
		public async Task Attach_WhenAttachingBaseEntity_DbSetAttachIsCalled()
		{
			// Arrange
			var mockDbSet = new Mock<DbSet<BaseEntity>>();
			var mockEntity = new Mock<BaseEntity>();
			var mockDbContext = new Mock<IDbContext>();

			var repo = CreateRepo(mockDbContext);

			mockDbContext.Setup(c => c.Set<BaseEntity>())
				.Returns(mockDbSet.Object);

			// Act
			await repo.Attach(mockEntity.Object);

			Assert.Multiple(() =>
			{
				mockDbSet.Verify(s => s.Attach(It.IsAny<BaseEntity>()), Times.Once, "Attach is only called once.");
				NotSaved(mockDbContext);
			});
		}

		[Test]
		public async Task Attach_WhenAttachingBaseEntity_LocalDbContextSetContainsEntity()
		{
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
			var testEntity = new TestEntity();

			await repo.Attach(testEntity);

			Assert.Multiple(() =>
			{
				CollectionAssert.Contains(context.Set<TestEntity>().Local, testEntity, "DbSet<TestEntity> should contain entity");
				CollectionAssert.DoesNotContain(context.Set<TestEntity>(), testEntity, "DbSet<TestEntity> should not contain entity");
			});
		}

		[Test]
		public async Task Attach_WhenAttachingBaseEntityCollection_DbSetAttachRangeIsCalled()
		{
			// Arrange
			var mockDbSet = new Mock<DbSet<BaseEntity>>();
			var mockEntities = new Mock<ICollection<BaseEntity>>();
			var mockDbContext = new Mock<IDbContext>();

			var repo = CreateRepo(mockDbContext);

			mockDbContext.Setup(c => c.Set<BaseEntity>())
				.Returns(mockDbSet.Object);

			// Act
			await repo.Attach(mockEntities.Object);

			Assert.Multiple(() =>
			{
				mockDbSet.Verify(s => s.AttachRange(It.IsAny<ICollection<BaseEntity>>()), Times.Once, "Attach is only called once.");
				NotSaved(mockDbContext);
			});
		}

		[Test]
		public async Task Attach_WhenAttachingBaseEntityCollection_LocalDbContextSetContainsEntities()
		{
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
			var testEntities = new List<TestEntity>
			{
				new TestEntity()
			};

			await repo.Attach(testEntities);

			Assert.Multiple(() =>
			{
				Assert.That(context.Set<TestEntity>().Local.Intersect(testEntities).Count() == testEntities.Count(), "DbSet<TestEntity> should contain entities");
				Assert.That(context.Set<TestEntity>().ToList().Intersect(testEntities).Count() == 0, "DbSet<TestEntity> should not contain entities");
			});
		}
	}
}