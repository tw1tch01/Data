using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using DevGate.Data.Contexts;
using DevGate.Data.Repositories;
using DevGate.Data.Specifications;
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
		public async Task AddAsync_WhenAddingBaseEntity_DbSetAddRangeAsyncIsCalled()
		{
			// Arrange
			var mockDbSet = new Mock<DbSet<BaseEntity>>();
			var mockEntity = new Mock<BaseEntity>();
			var mockDbContext = new Mock<IDbContext>();
			var repo = CreateRepo(mockDbContext);

			mockDbContext.Setup(c => c.Set<BaseEntity>())
				.Returns(mockDbSet.Object);

			// Act
			await repo.AddAsync(mockEntity.Object);

			// Assert
			Assert.Multiple(() =>
			{
				mockDbSet.Verify(s => s.AddAsync(It.IsAny<BaseEntity>(), It.IsAny<CancellationToken>()), Times.Once, "Add async method should be called.");
				NotSaved(mockDbContext);
			});
		}

		[Test]
		public async Task AddAsync_WhenAddingBaseEntityCollection_DbSetAddAsyncIsCalled()
		{
			// Arrange
			var mockDbSet = new Mock<DbSet<BaseEntity>>();
			var mockEntities = new Mock<ICollection<BaseEntity>>();
			var mockDbContext = new Mock<IDbContext>();
			var repo = CreateRepo(mockDbContext);

			mockDbContext.Setup(c => c.Set<BaseEntity>())
				.Returns(mockDbSet.Object);

			// Act
			await repo.AddAsync(mockEntities.Object);

			// Assert
			Assert.Multiple(() =>
			{
				mockDbSet.Verify(s => s.AddRangeAsync(mockEntities.Object, It.IsAny<CancellationToken>()), Times.Once, "Add range async method should be called.");
				NotSaved(mockDbContext);
			});
		}

		[Test]
		public async Task AddAsync_WhenAddingTestEntityCollection_LocalDbContextSetContainsEntities()
		{
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
			var testEntity = new TestEntity();

			await repo.AddAsync(testEntity);

			Assert.Multiple(() =>
			{
				CollectionAssert.Contains(context.Set<TestEntity>().Local, testEntity, "Local DbSet<TestCreatedEntity> should contain entity");
				CollectionAssert.DoesNotContain(context.Set<TestEntity>(), testEntity, "DbSet<TestCreatedEnity> should not contain entity");
			});
		}

		[Test]
		public async Task AddAsync_WhenAddingTestEntityCollection_LocalDbContextSetContainsEntity()
		{
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
			var testEntities = new List<TestEntity>
			{
				new TestEntity { Id = _fixture.Create<int>() },
				new TestEntity { Id = _fixture.Create<int>() }
			};

			await repo.AddAsync(testEntities);

			Assert.Multiple(() =>
			{
				Assert.That(context.Set<TestEntity>().Local.Intersect(testEntities).Count() == testEntities.Count(), "Local DbSet<TestEntity> should contain entity");
				Assert.That(context.Set<TestEntity>().ToList().Intersect(testEntities).Count() == 0, "DbSet<TestEntity> should not contain entity");
			});
		}

		#region SaveAsync

		//[Test]
		//public async Task AddAsync_WhenAddingICreatedEntity_AuditFieldsAreSet()
		//{
		//	var options = new DbContextOptionsBuilder<TestDbContext>()
		//		.UseInMemoryDatabase(databaseName: "TestCreatedEntity")
		//		.Options;

		//	using var context = new TestDbContext(options);

		//	var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
		//	var testCreatedEntity = new TestCreatedEntity();

		//	var createdBy = _fixture.Create<string>();
		//	var createdOn = _fixture.Create<DateTime>();

		//	await repo.AddAsync(testCreatedEntity, createdBy, createdOn);

		//	Assert.Multiple(() =>
		//	{
		//		Assert.AreEqual(testCreatedEntity.CreatedBy, createdBy, "CreatedBy property should be set.");
		//		Assert.AreEqual(testCreatedEntity.CreatedOn, createdOn, "CreatedOn property should be set");
		//	});
		//}

		//[Test]
		//public async Task AddAsync_WhenAddingICreatedEntity_DbSetAddAsyncAndICreatedCreateMethodAreCalled()
		//{
		//	// Arrange
		//	var mockDbSet = new Mock<DbSet<TestCreatedEntity>>();
		//	var mockCreatedEntity = new Mock<TestCreatedEntity>();
		//	var mockDbContext = new Mock<TestDbContext>();
		//	var repo = CreateRepo(mockDbContext);

		//	mockDbContext.Setup(c => c.Set<TestCreatedEntity>())
		//		.Returns(mockDbSet.Object);

		//	// Act
		//	await repo.AddAsync(mockCreatedEntity.Object, _fixture.Create<string>(), _fixture.Create<DateTime>());

		//	// Assert
		//	Assert.Multiple(() =>
		//	{
		//		mockCreatedEntity.Verify(ce => ce.Create(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once, "Create method should be called.");
		//		mockDbSet.Verify(s => s.AddAsync(mockCreatedEntity.Object, It.IsAny<CancellationToken>()), Times.Once, "Add range async method should be called.");
		//		NotSaved(mockDbContext);
		//	});
		//}

		//[Test]
		//public async Task AddAsync_WhenAddingICreatedEntity_LocalDbContextSetContainsEntity()
		//{
		//	var options = new DbContextOptionsBuilder<TestDbContext>()
		//		.UseInMemoryDatabase(databaseName: "TestCreatedEntity")
		//		.Options;

		//	using var context = new TestDbContext(options);

		//	var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
		//	var testCreatedEntity = new TestCreatedEntity();

		//	var createdBy = _fixture.Create<string>();
		//	var createdOn = _fixture.Create<DateTime>();

		//	await repo.AddAsync(testCreatedEntity, createdBy, createdOn);

		//	Assert.Multiple(() =>
		//	{
		//		CollectionAssert.Contains(context.Set<TestCreatedEntity>().Local, testCreatedEntity, "Local DbSet<TestCreatedEntity> should contain entity");
		//		CollectionAssert.DoesNotContain(context.Set<TestCreatedEntity>(), testCreatedEntity, "DbSet<TestCreatedEnity> should not contain entity");
		//	});
		//}

		//[Test]
		//public async Task AddAsync_WhenAddingICreatedEntityCollection_AuditFieldsAreSet()
		//{
		//	var options = new DbContextOptionsBuilder<TestDbContext>()
		//		.UseInMemoryDatabase(databaseName: "TestCreatedEntity")
		//		.Options;

		//	using var context = new TestDbContext(options);

		//	var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
		//	var testCreatedEntities = new List<TestCreatedEntity>
		//	{
		//		new TestCreatedEntity()
		//	};

		//	var createdBy = _fixture.Create<string>();
		//	var createdOn = _fixture.Create<DateTime>();

		//	await repo.AddAsync(testCreatedEntities, createdBy, createdOn);

		//	Assert.Multiple(() =>
		//	{
		//		Assert.That(testCreatedEntities.All(t => t.CreatedBy == createdBy), "CreatedBy property should be set.");
		//		Assert.That(testCreatedEntities.All(t => t.CreatedOn == createdOn), "CreatedOn property should be set");
		//	});
		//}

		//[Test]
		//public async Task AddAsync_WhenAddingICreatedEntityCollection_DbSetAddRangeAsyncAndICreatedCreateMethodAreCalled()
		//{
		//	// Arrange
		//	var mockDbSet = new Mock<DbSet<TestCreatedEntity>>();
		//	var mockCreatedEntity1 = new Mock<TestCreatedEntity>();
		//	var mockCreatedEntity2 = new Mock<TestCreatedEntity>();
		//	var createdEntities = new List<TestCreatedEntity> { mockCreatedEntity1.Object, mockCreatedEntity2.Object };

		//	var mockDbContext = new Mock<TestDbContext>();
		//	var repo = CreateRepo(mockDbContext);

		//	mockDbContext.Setup(c => c.Set<TestCreatedEntity>())
		//		.Returns(mockDbSet.Object);

		//	// Act
		//	await repo.AddAsync(createdEntities, _fixture.Create<string>(), _fixture.Create<DateTime>());

		//	// Assert
		//	Assert.Multiple(() =>
		//	{
		//		mockCreatedEntity1.Verify(ce => ce.Create(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once, "CreatedEntity1 Create() should be called.");
		//		mockCreatedEntity2.Verify(ce => ce.Create(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once, "CreatedEntity2 Create() should be called.");
		//		mockDbSet.Verify(s => s.AddRangeAsync(createdEntities, It.IsAny<CancellationToken>()), Times.Once, "Add range async method should be called.");
		//		NotSaved(mockDbContext);
		//	});
		//}

		//[Test]
		//public async Task AddAsync_WhenAddingICreatedEntityCollection_LocalDbContextSetContainsEntities()
		//{
		//	var options = new DbContextOptionsBuilder<TestDbContext>()
		//		.UseInMemoryDatabase(databaseName: "TestCreatedEntity")
		//		.Options;

		//	using var context = new TestDbContext(options);

		//	var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
		//	var testCreatedEntities = new List<TestCreatedEntity>
		//	{
		//		new TestCreatedEntity()
		//	};

		//	var createdBy = _fixture.Create<string>();
		//	var createdOn = _fixture.Create<DateTime>();

		//	await repo.AddAsync(testCreatedEntities, createdBy, createdOn);

		//	Assert.Multiple(() =>
		//	{
		//		Assert.That(context.Set<TestCreatedEntity>().Local.Intersect(testCreatedEntities).Count() == testCreatedEntities.Count(), "DbSet should contain entity collection");
		//		Assert.That(context.Set<TestCreatedEntity>().ToList().Intersect(testCreatedEntities).Count() == 0, "DbSet<TestCreatedEntity> should not contain entity collection");
		//	});
		//}

		#endregion SaveAsync
	}
}