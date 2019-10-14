using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using DevGate.Data.Contexts;
using DevGate.Domain.Entities;
using DevGate.Data.Repositories;
using DevGate.Data.Specifications;
using DevGate.Data.Tests.TestClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DevGate.Data.Tests.RepositoryTests
{
	[TestFixture]
	public class EntityRepositoryTests
	{
		private readonly Fixture _fixture = new Fixture();

		#region AddAsync

		[Test]
		public async Task AddAsync_WhenAddingBaseEntity_DbSetAddRangeAsyncIsCalled()
		{
			// Arrange
			var mockDbSet = new Mock<DbSet<BaseEntity>>();
			var mockEntity = new Mock<BaseEntity>();
			var mockDbContext = new Mock<TestDbContext>();
			var entityRepo = CreateRepo(mockDbContext);

			mockDbContext.Setup(c => c.Set<BaseEntity>())
				.Returns(mockDbSet.Object);

			// Act
			await entityRepo.AddAsync(mockEntity.Object);

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
			var mockDbContext = new Mock<TestDbContext>();
			var entityRepo = CreateRepo(mockDbContext);

			mockDbContext.Setup(c => c.Set<BaseEntity>())
				.Returns(mockDbSet.Object);

			// Act
			await entityRepo.AddAsync(mockEntities.Object);

			// Assert
			Assert.Multiple(() =>
			{
				mockDbSet.Verify(s => s.AddRangeAsync(mockEntities.Object, It.IsAny<CancellationToken>()), Times.Once, "Add range async method should be called.");
				NotSaved(mockDbContext);
			});
		}

		[Test]
		public async Task AddASync_WhenAddingICreatedEntity_AuditFieldsAreSet()
		{
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestCreatedEntity")
				.Options;

			using var context = new TestDbContext(options);

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
			var testCreatedEntity = new TestCreatedEntity();

			var createdBy = _fixture.Create<string>();
			var createdOn = _fixture.Create<DateTime>();

			await repo.AddAsync(testCreatedEntity, createdBy, createdOn);

			Assert.Multiple(() =>
			{
				Assert.AreEqual(testCreatedEntity.CreatedBy, createdBy, "CreatedBy property should be set.");
				Assert.AreEqual(testCreatedEntity.CreatedOn, createdOn, "CreatedOn property should be set");
			});
		}

		[Test]
		public async Task AddASync_WhenAddingICreatedEntity_DbSetAddAsyncAndICreatedCreateMethodAreCalled()
		{
			// Arrange
			var mockDbSet = new Mock<DbSet<TestCreatedEntity>>();
			var mockCreatedEntity = new Mock<TestCreatedEntity>();
			var mockDbContext = new Mock<TestDbContext>();
			var entityRepo = CreateRepo(mockDbContext);

			mockDbContext.Setup(c => c.Set<TestCreatedEntity>())
				.Returns(mockDbSet.Object);

			// Act
			await entityRepo.AddAsync(mockCreatedEntity.Object, _fixture.Create<string>(), _fixture.Create<DateTime>());

			// Assert
			Assert.Multiple(() =>
			{
				mockCreatedEntity.Verify(ce => ce.Create(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once, "Create method should be called.");
				mockDbSet.Verify(s => s.AddAsync(mockCreatedEntity.Object, It.IsAny<CancellationToken>()), Times.Once, "Add range async method should be called.");
				NotSaved(mockDbContext);
			});
		}

		[Test]
		public async Task AddASync_WhenAddingICreatedEntity_LocalDbContextSetContainsEntity()
		{
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestCreatedEntity")
				.Options;

			using var context = new TestDbContext(options);

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
			var testCreatedEntity = new TestCreatedEntity();

			var createdBy = _fixture.Create<string>();
			var createdOn = _fixture.Create<DateTime>();

			await repo.AddAsync(testCreatedEntity, createdBy, createdOn);

			Assert.Multiple(() =>
			{
				CollectionAssert.Contains(context.Set<TestCreatedEntity>().Local, testCreatedEntity, "Local DbSet<TestCreatedEntity> should contain entity");
				CollectionAssert.DoesNotContain(context.Set<TestCreatedEntity>(), testCreatedEntity, "DbSet<TestCreatedEnity> should not contain entity");
			});
		}

		[Test]
		public async Task AddASync_WhenAddingICreatedEntityCollection_AuditFieldsAreSet()
		{
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestCreatedEntity")
				.Options;

			using var context = new TestDbContext(options);

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
			var testCreatedEntities = new List<TestCreatedEntity>
			{
				new TestCreatedEntity()
			};

			var createdBy = _fixture.Create<string>();
			var createdOn = _fixture.Create<DateTime>();

			await repo.AddAsync(testCreatedEntities, createdBy, createdOn);

			Assert.Multiple(() =>
			{
				Assert.That(testCreatedEntities.All(t => t.CreatedBy == createdBy), "CreatedBy property should be set.");
				Assert.That(testCreatedEntities.All(t => t.CreatedOn == createdOn), "CreatedOn property should be set");
			});
		}

		[Test]
		public async Task AddASync_WhenAddingICreatedEntityCollection_DbSetAddRangeAsyncAndICreatedCreateMethodAreCalled()
		{
			// Arrange
			var mockDbSet = new Mock<DbSet<TestCreatedEntity>>();
			var mockCreatedEntity1 = new Mock<TestCreatedEntity>();
			var mockCreatedEntity2 = new Mock<TestCreatedEntity>();
			var createdEntities = new List<TestCreatedEntity> { mockCreatedEntity1.Object, mockCreatedEntity2.Object };

			var mockDbContext = new Mock<TestDbContext>();
			var entityRepo = CreateRepo(mockDbContext);

			mockDbContext.Setup(c => c.Set<TestCreatedEntity>())
				.Returns(mockDbSet.Object);

			// Act
			await entityRepo.AddAsync(createdEntities, _fixture.Create<string>(), _fixture.Create<DateTime>());

			// Assert
			Assert.Multiple(() =>
			{
				mockCreatedEntity1.Verify(ce => ce.Create(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once, "CreatedEntity1 Create() should be called.");
				mockCreatedEntity2.Verify(ce => ce.Create(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once, "CreatedEntity2 Create() should be called.");
				mockDbSet.Verify(s => s.AddRangeAsync(createdEntities, It.IsAny<CancellationToken>()), Times.Once, "Add range async method should be called.");
				NotSaved(mockDbContext);
			});
		}

		[Test]
		public async Task AddASync_WhenAddingICreatedEntityCollection_LocalDbContextSetContainsEntities()
		{
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestCreatedEntity")
				.Options;

			using var context = new TestDbContext(options);

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);
			var testCreatedEntities = new List<TestCreatedEntity>
			{
				new TestCreatedEntity()
			};

			var createdBy = _fixture.Create<string>();
			var createdOn = _fixture.Create<DateTime>();

			await repo.AddAsync(testCreatedEntities, createdBy, createdOn);

			Assert.Multiple(() =>
			{
				Assert.That(context.Set<TestCreatedEntity>().Local.Intersect(testCreatedEntities).Count() == testCreatedEntities.Count(), "DbSet should contain entity collection");
				Assert.That(context.Set<TestCreatedEntity>().ToList().Intersect(testCreatedEntities).Count() == 0, "DbSet<TestCreatedEntity> should not contain entity collection");
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

		#endregion AddAsync

		#region Attach

		[Test]
		public async Task Attach_WhenAttachingBaseEntity_DbSetAttachIsCalled()
		{
			// Arrange
			var mockDbSet = new Mock<DbSet<BaseEntity>>();
			var mockEntity = new Mock<BaseEntity>();
			var mockDbContext = new Mock<TestDbContext>();

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
			var mockDbContext = new Mock<TestDbContext>();

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

		#endregion Attach

		#region FindAsync

		//[Test]
		//public async Task FindAsync_ReturnsFirstEntityThatMatchesSpecification()
		//{
		//	//// Arrange
		//	//var mockDbSet = new Mock<DbSet<TestEntity>>();
		//	//var mockDbContext = new Mock<TestDbContext>();
		//	//var entityRepo = CreateRepo(mockDbContext);
		//	//var mockSpecification = new Mock<Specification<TestEntity>>();

		//	//var testEntity = new TestEntity { Id = _fixture.Create<int>() };

		//	//Expression<Func<TestEntity, bool>> expression = entity => true;

		//	//mockSpecification.Setup(s => s.ToExpression())
		//	//	.Returns(expression);

		//	//mockDbContext.Setup(c => c.Set<TestEntity>())
		//	//	.Returns(mockDbSet.Object);

		//	//// Act
		//	//var entity = await entityRepo.FindAsync(mockSpecification.Object);
		//	Assert.Pass();
		//}

		[Test]
		public async Task FindAsync_ReturnsFirstEntityThatMatchesSpecification()
		{
			var id = _fixture.Create<int>();
			var mockSpecification = new Mock<Specification<TestEntity>>();
			var testEntity = new TestEntity { Id = id };
			Expression<Func<TestEntity, bool>> expression = entity => entity.Id == id;

			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(databaseName: "TestEntity")
				.Options;

			using var context = new TestDbContext(options);

			context.TestEntities.Add(testEntity);
			context.TestEntities.Add(new TestEntity { Id = _fixture.Create<int>() });
			context.SaveChanges();

			mockSpecification.Setup(s => s.ToExpression())
				.Returns(expression);

			var repo = new EntityRepository<TestDbContext>(context, new Mock<ILogger<TestDbContext>>().Object);

			var testspec = new TestEntityFindById(id);

			var foundEntity = await repo.FindAsync(testspec);

			Assert.Multiple(() =>
			{
				Assert.AreEqual(testEntity.Id, foundEntity.Id);
			});
		}

		#endregion FindAsync

		private EntityRepository<TContext> CreateRepo<TContext>(Mock<TContext> mockContext = null, Mock<ILogger<TContext>> mockLogger = null) where TContext : DbContext, IDbContext
		{
			mockContext ??= new Mock<TContext>();
			mockLogger ??= new Mock<ILogger<TContext>>();

			return new EntityRepository<TContext>(mockContext.Object, mockLogger.Object);
		}

		private void NotSaved<TContext>(Mock<TContext> mockDbContext) where TContext : DbContext, IDbContext
		{
			mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never, "Context should never saved.");
			mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never, "Context should never saved.");
			mockDbContext.Verify(c => c.SaveChanges(), Times.Never, "Context should never saved.");
		}
	}
}