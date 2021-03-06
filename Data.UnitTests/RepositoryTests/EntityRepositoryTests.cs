﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Contexts;
using Data.Repositories;
using Data.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Data.UnitTests.RepositoryTests
{
    [TestFixture]
    public class EntityRepositoryTests
    {
        #region AddAsync

        [Test]
        public void AddAsync_WhenObjectIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddAsync<object>(null));
            Assert.AreEqual("Value cannot be null. (Parameter 'entity')", exception.Message);
        }

        [Test]
        public async Task AddAsync_WhenAddingObject_VerifyDbSetAddAsyncIsCalled()
        {
            var @object = new object();
            var mockRepository = new Mock<IAuditedContext>();
            var mockDbSet = new Mock<DbSet<object>>();

            mockRepository.Setup(a => a.Set<object>()).Returns(mockDbSet.Object);

            var repository = new EntityRepository<IAuditedContext>(mockRepository.Object, It.IsAny<ILogger<IAuditedContext>>());

            await repository.AddAsync(@object);

            Assert.Multiple(() =>
            {
                mockDbSet.Verify(a => a.AddAsync(@object, It.IsAny<CancellationToken>()), Times.Once);
                mockRepository.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never, "Context should never saved.");
            });
        }

        [Test]
        public void AddRangeAsync_WhenObjectCollectionIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddRangeAsync<object>(null));
            Assert.AreEqual("Value cannot be null. (Parameter 'entities')", exception.Message);
        }

        [Test]
        public void AddRangeAsync_WhenObjectCollectionIsEmpty_ThrowsArgumentException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.ThrowsAsync<ArgumentException>(() => repository.AddRangeAsync(new List<object>()));
            Assert.AreEqual("Cannot add an empty collection. (Parameter 'entities')", exception.Message);
        }

        [Test]
        public async Task AddRangeAsync_WhenAddingCollection_VerifyDbSetAddRangeAsyncIsCalled()
        {
            var objects = new List<object> { new object() };
            var mockRepository = new Mock<IAuditedContext>();
            var mockDbSet = new Mock<DbSet<object>>();

            mockRepository.Setup(a => a.Set<object>()).Returns(mockDbSet.Object);

            var repository = new EntityRepository<IAuditedContext>(mockRepository.Object, It.IsAny<ILogger<IAuditedContext>>());

            await repository.AddRangeAsync(objects);

            Assert.Multiple(() =>
            {
                mockDbSet.Verify(a => a.AddRangeAsync(objects, It.IsAny<CancellationToken>()), Times.Once);
                mockRepository.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never, "Context should never saved.");
            });
        }

        #endregion AddAsync

        #region Attach

        [Test]
        public void Attach_WhenObjectIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.Throws<ArgumentNullException>(() => repository.Attach<object>(null));
            Assert.AreEqual("Value cannot be null. (Parameter 'entity')", exception.Message);
        }

        [Test]
        public void Attach_WhenAttachingObject_VerifyDbSetAttachIsCalled()
        {
            var @object = new object();
            var mockRepository = new Mock<IAuditedContext>();
            var mockDbSet = new Mock<DbSet<object>>();

            mockRepository.Setup(a => a.Set<object>()).Returns(mockDbSet.Object);

            var repository = new EntityRepository<IAuditedContext>(mockRepository.Object, It.IsAny<ILogger<IAuditedContext>>());

            repository.Attach(@object);

            Assert.Multiple(() =>
            {
                mockDbSet.Verify(a => a.Attach(@object), Times.Once);
                mockRepository.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never, "Context should never saved.");
            });
        }

        [Test]
        public void AttachRange_WhenObjectCollectionIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.Throws<ArgumentNullException>(() => repository.AttachRange<object>(null));
            Assert.AreEqual("Value cannot be null. (Parameter 'entities')", exception.Message);
        }

        [Test]
        public void AttachRange_WhenObjectCollectionIsEmpty_ThrowsArgumentException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.Throws<ArgumentException>(() => repository.AttachRange(new List<object>()));
            Assert.AreEqual("Cannot attach an empty collection. (Parameter 'entities')", exception.Message);
        }

        [Test]
        public void AttachRange_WhenAttachingCollection_VerifyDbSetAttachRangeIsCalled()
        {
            var objects = new List<object> { new object() };
            var mockRepository = new Mock<IAuditedContext>();
            var mockDbSet = new Mock<DbSet<object>>();

            mockRepository.Setup(a => a.Set<object>()).Returns(mockDbSet.Object);

            var repository = new EntityRepository<IAuditedContext>(mockRepository.Object, It.IsAny<ILogger<IAuditedContext>>());

            repository.AttachRange(objects);

            Assert.Multiple(() =>
            {
                mockDbSet.Verify(a => a.AttachRange(objects), Times.Once);
                mockRepository.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never, "Context should never saved.");
            });
        }

        #endregion Attach

        #region FindByPrimaryKeyAsync

        [Test]
        public void FindByPrimaryKeyAsync_WhenPrimaryKeyIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.FindByPrimaryKeyAsync<object, object>(null));
            Assert.AreEqual("Value cannot be null. (Parameter 'primaryKey')", exception.Message);
        }

        [Test]
        public async Task FindByPrimaryKeyAsync_VerifyDbSetFindAsyncIsCalled()
        {
            var mockRepository = new Mock<IAuditedContext>();
            var mockDbSet = new Mock<DbSet<object>>();

            mockRepository.Setup(a => a.Set<object>()).Returns(mockDbSet.Object);

            var repository = new EntityRepository<IAuditedContext>(mockRepository.Object, It.IsAny<ILogger<IAuditedContext>>());

            await repository.FindByPrimaryKeyAsync<object, int>(1);

            mockDbSet.Verify(a => a.FindAsync(1), Times.Once);
        }

        #endregion FindByPrimaryKeyAsync

        #region GetAsync

        [Test]
        public void GetAsync_WhenSpecificationIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.GetAsync<object>(null));
            Assert.AreEqual("Value cannot be null. (Parameter 'specification')", exception.Message);
        }

        #endregion GetAsync

        #region SingleAsync

        [Test]
        public void SingleAsync_WhenSpecificationIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.SingleAsync<object>(null));
            Assert.AreEqual("Value cannot be null. (Parameter 'specification')", exception.Message);
        }

        #endregion SingleAsync

        #region ListAsync

        [Test]
        public void ListAsync_WhenSpecificationIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.ListAsync<object>(null));
            Assert.AreEqual("Value cannot be null. (Parameter 'specification')", exception.Message);
        }

        #endregion ListAsync

        #region PagedListAsync

        [Test]
        public void PagedListAsync_WhenSpecificationIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.PagedListAsync<object>(0, 0, null));
            Assert.AreEqual("Value cannot be null. (Parameter 'specification')", exception.Message);
        }

        [Test]
        public void PagedListAsync_WhenPageIsLessThanZero_ThrowsArgumentException()
        {
            var mockSpecification = new Mock<LinqSpecification<object>>();
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.PagedListAsync(-1, 0, mockSpecification.Object));
            Assert.AreEqual("Specified argument was out of the range of valid values. (Parameter 'page')", exception.Message);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void PagedListAsync_WhenPageSizeIsLessThanOrEqualToZero_ThrowsArgumentException(int pageSize)
        {
            var mockSpecification = new Mock<LinqSpecification<object>>();
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.PagedListAsync(0, pageSize, mockSpecification.Object));
            Assert.AreEqual("Specified argument was out of the range of valid values. (Parameter 'pageSize')", exception.Message);
        }

        #endregion PagedListAsync

        #region QueryAsync

        [Test]
        public void QueryAsync_WhenSpecificationIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var resolver = It.IsAny<Func<IQueryable<object>, Task<object>>>();
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.QueryAsync(null, resolver));
            Assert.AreEqual("Value cannot be null. (Parameter 'specification')", exception.Message);
        }

        [Test]
        public void QueryAsync_WhenResolverIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var mockLinqSpecificaiton = new Mock<LinqSpecification<object>>();
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.QueryAsync<object, object>(mockLinqSpecificaiton.Object, null));
            Assert.AreEqual("Value cannot be null. (Parameter 'resolver')", exception.Message);
        }

        #endregion QueryAsync

        #region Remove

        [Test]
        public void Remove_WhenObjectIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.Throws<ArgumentNullException>(() => repository.Remove((object)null));
            Assert.AreEqual("Value cannot be null. (Parameter 'entity')", exception.Message);
        }

        [Test]
        public void Remove_WhenObjectCollectionIsNull_ThrowsArgumentNullException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.Throws<ArgumentNullException>(() => repository.RemoveRange((ICollection<object>)null));
            Assert.AreEqual("Value cannot be null. (Parameter 'entities')", exception.Message);
        }

        [Test]
        public void Remove_WhenObjectCollectionIsEmpty_ThrowsArgumentException()
        {
            var repository = new EntityRepository<IAuditedContext>(It.IsAny<IAuditedContext>(), It.IsAny<ILogger<IAuditedContext>>());
            var exception = Assert.Throws<ArgumentException>(() => repository.RemoveRange<object>(new List<object>()));
            Assert.AreEqual("Cannot remove an empty collection. (Parameter 'entities')", exception.Message);
        }

        [Test]
        public void Remove_WhenRemovingObject_VerifyDbSetRemoveIsCalled()
        {
            var @object = new object();
            var mockRepository = new Mock<IAuditedContext>();
            var mockDbSet = new Mock<DbSet<object>>();

            mockRepository.Setup(a => a.Set<object>()).Returns(mockDbSet.Object);

            var repository = new EntityRepository<IAuditedContext>(mockRepository.Object, It.IsAny<ILogger<IAuditedContext>>());

            repository.Remove(@object);

            Assert.Multiple(() =>
            {
                mockDbSet.Verify(a => a.Remove(@object), Times.Once);
                mockRepository.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never, "Context should never saved.");
            });
        }

        [Test]
        public void RemoveRange_WhenRemovingCollection_VerifyDbSetRemoveRangeIsCalled()
        {
            var objects = new List<object> { new object() };
            var mockRepository = new Mock<IAuditedContext>();
            var mockDbSet = new Mock<DbSet<object>>();

            mockRepository.Setup(a => a.Set<object>()).Returns(mockDbSet.Object);

            var repository = new EntityRepository<IAuditedContext>(mockRepository.Object, It.IsAny<ILogger<IAuditedContext>>());

            repository.RemoveRange(objects);

            Assert.Multiple(() =>
            {
                mockDbSet.Verify(a => a.RemoveRange(objects), Times.Once);
                mockRepository.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never, "Context should never saved.");
            });
        }

        #endregion Remove
    }
}