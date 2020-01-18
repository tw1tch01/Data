using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoFixture;
using Data.Contexts;
using Data.Repositories;
using Data.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Data.IntegrationTests.RepositoryTests
{
    [TestFixture]
    internal class ContextRepositoryTests : MemoryDbSetupFixture
    {
        private readonly Fixture _fixture = new Fixture();
        private AuditedEntity _entity;

        [SetUp]
        public async Task Init()
        {
            _entity = new AuditedEntity { Id = Guid.NewGuid(), CreatedBy = _fixture.Create<string>(), CreatedProcess = _fixture.Create<string>(), CreatedOn = DateTime.UtcNow, Name = _fixture.Create<string>() };

            _memoryDb.Context.Add(_entity);
            await _memoryDb.Context.SaveChangesAsync();
        }

        #region AddAsync

        [Test]
        public async Task AddAsync_WhenAddingEntity_VerifyDbSetContainsAddedEntity()
        {
            var auditEntity = new AuditedEntity();

            await _memoryDb.Repository.AddAsync(auditEntity);

            Assert.Multiple(() =>
            {
                CollectionAssert.Contains(_memoryDb.Context.Set<AuditedEntity>().Local, auditEntity);
                CollectionAssert.DoesNotContain(_memoryDb.Context.Set<AuditedEntity>(), auditEntity);
            });
        }

        #endregion AddAsync

        #region AddRangeAsync

        [Test]
        public async Task AddRangeAsync_WhenAddingCollection_VerifyDbSetContainsAddedCollection()
        {
            var auditEntities = new List<AuditedEntity> { new AuditedEntity() };

            await _memoryDb.Repository.AddRangeAsync(auditEntities);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(auditEntities.Count(), _memoryDb.Context.Set<AuditedEntity>().Local.Intersect(auditEntities).Count());
                Assert.AreEqual(0, _memoryDb.Context.Set<AuditedEntity>().AsEnumerable().Intersect(auditEntities).Count());
            });
        }

        #endregion AddRangeAsync

        #region Attach

        [Test]
        public void Attach_WhenAttachingEntity_VerifyDbSetContainsEntity()
        {
            var auditEntity = new AuditedEntity();

            _memoryDb.Repository.Attach(auditEntity);

            Assert.Multiple(() =>
            {
                CollectionAssert.Contains(_memoryDb.Context.Set<AuditedEntity>().Local, auditEntity);
                CollectionAssert.DoesNotContain(_memoryDb.Context.Set<AuditedEntity>(), auditEntity);
            });
        }

        #endregion Attach

        #region AttachRange

        [Test]
        public void AttachRange_WhenAttachingCollection_VerifyDbSetContainsCollection()
        {
            var auditEntities = new List<AuditedEntity> { new AuditedEntity() };

            _memoryDb.Repository.AttachRange(auditEntities);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(auditEntities.Count(), _memoryDb.Context.Set<AuditedEntity>().Local.Intersect(auditEntities).Count());
                Assert.AreEqual(0, _memoryDb.Context.Set<AuditedEntity>().AsEnumerable().Intersect(auditEntities).Count());
            });
        }

        #endregion AttachRange

        #region FindByPrimaryKeyAsync

        [Test]
        public async Task FindByPrimaryKeyAsync_WhenEntityPrimaryKeyMatchesValuePassedIn_ReturnsEntity()
        {
            var entity = await _memoryDb.Repository.FindByPrimaryKeyAsync<AuditedEntity, Guid>(_entity.Id);

            Assert.AreEqual(_entity, entity);
        }

        [Test]
        public async Task FindByPrimaryKeyAsync_WhenEntityPrimaryKeyDoesNotMatchValuePassedIn_ReturnsNull()
        {
            var entity = await _memoryDb.Repository.FindByPrimaryKeyAsync<AuditedEntity, Guid>(Guid.NewGuid());

            Assert.IsNull(entity);
        }

        [Test]
        public async Task FindByPrimaryKeyAsync_WhenContextHasNoEntities_ReturnsNull()
        {
            using (var context = new MemoryContext())
            {
                var repo = new ContextRepository<MemoryContext>(context, It.IsAny<ILogger<IAuditedContext>>());

                var entity = await repo.FindByPrimaryKeyAsync<AuditedEntity, Guid>(It.IsAny<Guid>());

                Assert.IsNull(entity);
            }
        }

        #endregion FindByPrimaryKeyAsync

        #region GetAsync

        [Test]
        public async Task GetAsync_WhenEntityMatchesSpecification_ReturnsEntity()
        {
            var createdBy = _fixture.Create<string>();
            var auditEntity = new AuditedEntity
            {
                CreatedBy = createdBy
            };

            await _memoryDb.Context.AddAsync(auditEntity);
            await _memoryDb.Context.SaveChangesAsync();

            var entity = await _memoryDb.Repository.GetAsync(new CreatedBySpec(createdBy));

            Assert.AreEqual(auditEntity, entity);
        }

        [Test]
        public async Task GetAsync_WhenMultipleEntitesMatchSpecification_ReturnsFirstEntityFound()
        {
            var createdBy = _fixture.Create<string>();
            var firstEntity = new AuditedEntity { CreatedBy = createdBy };
            var secondEntity = new AuditedEntity { CreatedBy = createdBy };

            await _memoryDb.Context.Set<AuditedEntity>().AddAsync(firstEntity);
            await _memoryDb.Context.SaveChangesAsync();
            await _memoryDb.Context.Set<AuditedEntity>().AddAsync(secondEntity);
            await _memoryDb.Context.SaveChangesAsync();

            var entity = await _memoryDb.Repository.GetAsync(new CreatedBySpec(createdBy));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(firstEntity, entity);
                Assert.AreNotEqual(secondEntity, entity);
            });
        }

        [Test]
        public async Task GetAsync_WhenEntityDoesNotMatchSpecification_ReturnsNull()
        {
            var createdBy = _fixture.Create<string>();
            var auditEntity = new AuditedEntity
            {
                CreatedBy = _fixture.Create<string>()
            };

            _memoryDb.Context.Set<AuditedEntity>().Add(auditEntity);
            _memoryDb.Context.SaveChanges();

            var entity = await _memoryDb.Repository.GetAsync(new CreatedBySpec(createdBy));

            Assert.IsNull(entity);
        }

        #endregion GetAsync

        #region SingleAsync

        [Test]
        public async Task SingleAsync_WhenEntityMatchesSpecification_ReturnsEntity()
        {
            var entity = await _memoryDb.Repository.SingleAsync(new CreatedBySpec(_entity.CreatedBy));

            Assert.AreEqual(_entity, entity);
        }

        [Test]
        public async Task SingleAsync_WhenEntityDoesNotMatchSpecification_ReturnsNull()
        {
            var entity = await _memoryDb.Repository.SingleAsync(new CreatedBySpec(_fixture.Create<string>()));

            Assert.IsNull(entity);
        }

        [Test]
        public async Task SingleAsync_WhenMoreThanOneEntityMatchesSpecification_ThrowsInvalidOperationException()
        {
            var createdBy = _fixture.Create<string>();
            var firstEntity = new AuditedEntity { Id = Guid.NewGuid(), CreatedBy = createdBy };
            var secondEntity = new AuditedEntity { Id = Guid.NewGuid(), CreatedBy = createdBy };

            await _memoryDb.Context.Set<AuditedEntity>().AddAsync(firstEntity);
            await _memoryDb.Context.Set<AuditedEntity>().AddAsync(secondEntity);
            await _memoryDb.Context.SaveChangesAsync();

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _memoryDb.Repository.SingleAsync(new CreatedBySpec(createdBy)));
        }

        #endregion SingleAsync

        #region ListAsync

        [Test]
        public async Task ListAsync_WhenEntityMatchesSpecification_ReturnsEntity()
        {
            var entities = await _memoryDb.Repository.ListAsync(new CreatedBySpec(_entity.CreatedBy));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, entities.Count);
                Assert.AreEqual(_entity, entities.First());
            });
        }

        [Test]
        public async Task ListAsync_WhenMultipleEntitesMatchSpecification_ReturnsAllEntities()
        {
            var createdBy = _fixture.Create<string>();
            var firstEntity = new AuditedEntity { CreatedBy = createdBy };
            var secondEntity = new AuditedEntity { CreatedBy = createdBy };

            await _memoryDb.Context.Set<AuditedEntity>().AddAsync(firstEntity);
            await _memoryDb.Context.Set<AuditedEntity>().AddAsync(secondEntity);
            await _memoryDb.Context.SaveChangesAsync();

            var entities = await _memoryDb.Repository.ListAsync(new CreatedBySpec(createdBy));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, entities.Count);
                CollectionAssert.Contains(entities, firstEntity);
                CollectionAssert.Contains(entities, secondEntity);
            });
        }

        [Test]
        public async Task ListAsync_WhenMultipleEntitesMatchSpecification_WithOrderBy_ReturnsAllEntities()
        {
            var createdBy = _fixture.Create<string>();
            var firstEntity = new AuditedEntity { CreatedBy = createdBy, Name = "banana" };
            var secondEntity = new AuditedEntity { CreatedBy = createdBy, Name = "apple" };

            await _memoryDb.Context.Set<AuditedEntity>().AddAsync(firstEntity);
            await _memoryDb.Context.Set<AuditedEntity>().AddAsync(secondEntity);
            await _memoryDb.Context.SaveChangesAsync();

            var entities = await _memoryDb.Repository.ListAsync(new CreatedBySpec(createdBy).OrderByDescending(a => a.Name));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, entities.Count);
                CollectionAssert.Contains(entities, firstEntity);
                CollectionAssert.Contains(entities, secondEntity);
            });
        }

        [Test]
        public async Task ListAsync_WhenEntitesDoNotMatchSpecification_ReturnsEmptyCollection()
        {
            var createdBy = _fixture.Create<string>();
            var entities = await _memoryDb.Repository.ListAsync(new CreatedBySpec(createdBy));

            Assert.IsEmpty(entities);
        }

        #endregion ListAsync

        #region PagedListAsync

        [Test]
        public async Task PagedListAsync_WhenEntitiesMatchSpecification_ReturnsPagedResult()
        {
            var pagedResults = await _memoryDb.Repository.PagedListAsync(0, 10, new PlainSpec());

            Assert.Multiple(() =>
            {
                Assert.AreEqual(0, pagedResults.Page);
                Assert.AreEqual(10, pagedResults.PageSize);
                Assert.AreEqual(10, pagedResults.Items.Count);
            });
        }
        [Test]
        public async Task PagedListAsync_WhenEntitiesMatchSpecification_WhenSelector_ReturnsPagedResult()
        {
            var pagedResults = await _memoryDb.Repository.PagedListAsync(0, 10, new PlainSpec(), a => a.Id);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(0, pagedResults.Page);
                Assert.AreEqual(10, pagedResults.PageSize);
                Assert.AreEqual(10, pagedResults.Items.Count);
            });
        }

        #endregion PagedListAsync

        #region QueryAsync

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task QueryAsync_Selection(int N)
        {
            var createdBy = _fixture.Create<string>();
            var entities = new List<AuditedEntity>();
            for (int i = 0; i < N; i++)
            {
                entities.Add(new AuditedEntity { CreatedBy = createdBy });
            }

            Task<List<string>> resolver(IQueryable<AuditedEntity> a) => a.Select(b => b.CreatedBy).ToListAsync();

            await _memoryDb.Context.Set<AuditedEntity>().AddRangeAsync(entities);
            _memoryDb.Context.SaveChanges();

            var result = await _memoryDb.Repository.QueryAsync(new CreatedBySpec(createdBy), resolver);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(N, result.Count);
                Assert.That(result.All(a => a == createdBy));
            });
        }

        #endregion QueryAsync

        #region Remove

        [Test]
        public void Remove_WhenRemovingEntity_VerifyDbSetDoesNotContainRemovedEntity()
        {
            var auditEntity = new AuditedEntity();

            _memoryDb.Context.Add(auditEntity);
            _memoryDb.Context.SaveChanges();

            _memoryDb.Repository.Remove(auditEntity);

            Assert.Multiple(() =>
            {
                CollectionAssert.DoesNotContain(_memoryDb.Context.Set<AuditedEntity>().Local, auditEntity);
                CollectionAssert.Contains(_memoryDb.Context.Set<AuditedEntity>(), auditEntity);
            });
        }

        #endregion Remove

        #region RemoveRange

        [Test]
        public void RemoveRange_WhenRemovingEntity_VerifyDbSetDoesNotContainRemovedEntity()
        {
            var auditEntities = new List<AuditedEntity> { new AuditedEntity() };

            _memoryDb.Context.AddRange(auditEntities);
            _memoryDb.Context.SaveChanges();

            _memoryDb.Repository.RemoveRange(auditEntities);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(auditEntities.Count(), _memoryDb.Context.Set<AuditedEntity>().AsEnumerable().Intersect(auditEntities).Count());
                Assert.AreEqual(0, _memoryDb.Context.Set<AuditedEntity>().Local.Intersect(auditEntities).Count());
            });
        }

        #endregion RemoveRange

        internal class PlainSpec : LinqSpecification<AuditedEntity>
        {
            public override Expression<Func<AuditedEntity, bool>> AsExpression()
            {
                return a => true;
            }
        }

        internal class CreatedBySpec : LinqSpecification<AuditedEntity>
        {
            private readonly string _createdBy;

            public CreatedBySpec(string createdBy)
            {
                _createdBy = createdBy;
            }

            public override Expression<Func<AuditedEntity, bool>> AsExpression()
            {
                return a => a.CreatedBy == _createdBy;
            }
        }
    }
}