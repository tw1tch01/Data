using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using DevGate.Data.Repositories;
using DevGate.Data.Specifications;
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
		public async Task GetAsync_WhenSpecificationDoesNotMatchEntity_ReturnsNull()
		{
			// Arrange
			var mockSpecification = new Mock<Specification<TestEntity>>();
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			context.Add(new TestEntity { Id = _fixture.Create<int>() });
			context.SaveChanges();

			var id = _fixture.Create<int>();

			mockSpecification.Setup(s => s.Evaluate())
				.Returns(entity => entity.Id == id);

			mockSpecification.Setup(s => s.AsQueryable(context))
				.Returns(context.TestEntities.Where(e => e.Id == id).AsQueryable());

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);

			// Act
			var foundEntity = await repo.GetAsync(mockSpecification.Object);

			// Assert
			Assert.IsNull(foundEntity);
		}

		[Test]
		public async Task GetAsync_WhenSpecificationMatchesEntity_ReturnsFirstEntityThatMatchesSpecification()
		{
			// Arrange
			var id = _fixture.Create<int>();
			var testEntity = new TestEntity { Id = id };
			var mockSpecification = new Mock<Specification<TestEntity>>();

			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			context.Add(testEntity);
			context.SaveChanges();

			mockSpecification.Setup(s => s.Evaluate())
				.Returns(entity => entity.Id == id);

			mockSpecification.Setup(s => s.AsQueryable(context))
				.Returns(context.Set<TestEntity>().Where(e => e.Id == id).AsQueryable());

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);

			// Act
			var foundEntity = await repo.GetAsync(mockSpecification.Object);

			// Assert
			Assert.AreEqual(testEntity, foundEntity);
		}
	}
}