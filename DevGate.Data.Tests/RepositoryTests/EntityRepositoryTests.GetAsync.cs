using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using DevGate.Data.Repositories;
using DevGate.Data.Specifications;
using DevGate.Data.Tests.TestClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
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
			var testEntity = new TestEntity { Id = _fixture.Create<int>() };
			var mockSpecification = new Mock<Specification<TestEntity>>();
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			context.TestEntities.Add(new TestEntity { Id = _fixture.Create<int>() });
			context.SaveChanges();

			var mockTestEntities = context.TestEntities.AsQueryable().BuildMock();

			mockSpecification.Setup(s => s.Evaluate())
				.Returns(entity => entity.Id == testEntity.Id);

			mockSpecification.Setup(s => s.AsQueryable(context))
				.Returns(mockTestEntities.Object);

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
			var testEntity = new TestEntity { Id = _fixture.Create<int>() };
			var mockSpecification = new Mock<Specification<TestEntity>>();

			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			context.TestEntities.Add(testEntity);
			context.SaveChanges();

			var mockTestEntities = context.TestEntities.AsQueryable().BuildMock();

			mockSpecification.Setup(s => s.Evaluate())
				.Returns(entity => entity.Id == testEntity.Id);

			mockSpecification.Setup(s => s.AsQueryable(context))
				.Returns(mockTestEntities.Object);

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);

			// Act
			var foundEntity = await repo.GetAsync(mockSpecification.Object);

			// Assert
			Assert.AreEqual(testEntity, foundEntity);
		}
	}
}