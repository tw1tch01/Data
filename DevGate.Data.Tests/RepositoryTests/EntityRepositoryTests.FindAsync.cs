using System.Threading.Tasks;
using AutoFixture;
using DevGate.Data.Repositories;
using DevGate.Data.Tests.TestClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DevGate.Data.Tests.RepositoryTests
{
	public partial class EntityRepositoryTests
	{
		[Test]
		public async Task FindAsync_WhenEntityWithPrimaryKeyExists_ReturnsFoundEntity()
		{
			// Arrange
			var id = _fixture.Create<int>();
			var entity = new TestEntity { Id = id };
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			context.Add(entity);
			context.SaveChanges();

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);

			// Act
			var foundEntity = await repo.FindAsync<TestEntity, int>(id);

			// Assert
			Assert.AreEqual(entity, foundEntity);
		}

		[Test]
		public async Task FindAsync_WhenEntityWithPrimaryKeyExists_ReturnsNull()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			context.Add(new TestEntity { Id = _fixture.Create<int>() });
			context.SaveChanges();

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);

			// Act
			var foundEntity = await repo.FindAsync<TestEntity, int>(_fixture.Create<int>());

			// Assert
			Assert.IsNull(foundEntity);
		}
	}
}