using System;
using System.Linq;
using Data.Specifications;
using Moq;
using NUnit.Framework;

namespace Data.UnitTests.Specifications
{
    [TestFixture]
    public class LinqSpecificationTests
    {
        #region AsExpression

        public int MyProperty { get; set; }

        #endregion AsExpression

        #region IsSatisfiedBy

        public int MyProperty1 { get; set; }

        #endregion IsSatisfiedBy

        #region AddQuery

        [Test]
        public void AddQuery_AddsQueryToPreModifiersCollection()
        {
            var query = It.IsAny<Func<IQueryable<string>, IQueryable<string>>>();
            var mockSpecification = new Mock<LinqSpecification<string>>();

            mockSpecification.Object.AddQuery(query);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, mockSpecification.Object.Modifiers.Count);
                Assert.Contains(query, mockSpecification.Object.Modifiers.ToList());
            });
        }

        #endregion AddQuery

        #region Distinct

        [Test]
        public void Distinct_PreModifiersOnlyHasOneQuery()
        {
            var mockSpecification = new Mock<LinqSpecification<string>>();

            mockSpecification.Object.Distinct();

            Assert.AreEqual(1, mockSpecification.Object.Modifiers.Count);
        }

        #endregion Distinct

        #region DistinctBy

        [Test]
        public void DistinctBy_AddsQueryToPreModifiersCollection()
        {
            var mockSpecification = new Mock<LinqSpecification<string>>();

            mockSpecification.Object.DistinctBy(s => s.Length);

            Assert.AreEqual(1, mockSpecification.Object.Modifiers.Count);
        }

        #endregion DistinctBy

        #region Include

        [Test]
        public void Include_AddsQueryToPreModifiersCollection()
        {
            var mockSpecification = new Mock<LinqSpecification<string>>();

            mockSpecification.Object.Include(s => s.Length);

            Assert.AreEqual(1, mockSpecification.Object.Modifiers.Count);
        }

        #endregion Include

        #region OrderBy

        [Test]
        public void OrderBy_AddsQueryToPreModifiersCollection()
        {
            var mockSpecification = new Mock<LinqSpecification<string>>();

            mockSpecification.Object.OrderBy(s => s.Length);

            Assert.AreEqual(1, mockSpecification.Object.Modifiers.Count);
        }

        [Test]
        public void OrderBy_MultipleOrderByAddsThenBysToPreModifersInOrder()
        {
            var mockSpecification = new Mock<LinqSpecification<string>>();

            mockSpecification.Object.OrderBy(s => s.Length);
            mockSpecification.Object.OrderBy(s => s.Length);

            Assert.AreEqual(2, mockSpecification.Object.Modifiers.Count);
            //Assert.AreEqual(typeof(Func<IOrderedQueryable<string>, IQueryable<string>>), mockSpecification.Object.PreModifers.First().GetType());
            //Assert.AreEqual(typeof(Func<IOrderedQueryable<string>, IQueryable<string>>), mockSpecification.Object.PreModifers.Last().GetType());
        }

        #endregion OrderBy

        #region OrderByDescending

        [Test]
        public void OrderByDescending_AddsQueryToPreModifiersCollection()
        {
            var mockSpecification = new Mock<LinqSpecification<string>>();

            mockSpecification.Object.OrderByDescending(s => s.Length);

            Assert.AreEqual(1, mockSpecification.Object.Modifiers.Count);
        }

        [Test]
        public void OrderByDescending_MultipleOrderByAddsThenBysToPreModifersInOrder()
        {
            var mockSpecification = new Mock<LinqSpecification<string>>();

            mockSpecification.Object.OrderByDescending(s => s.Length);
            mockSpecification.Object.OrderByDescending(s => s.Length);

            Assert.AreEqual(2, mockSpecification.Object.Modifiers.Count);
            //Assert.AreEqual(typeof(Func<IOrderedQueryable<string>, IQueryable<string>>), mockSpecification.Object.PreModifers.First().GetType());
            //Assert.AreEqual(typeof(Func<IOrderedQueryable<string>, IQueryable<string>>), mockSpecification.Object.PreModifers.Last().GetType());
        }

        #endregion OrderByDescending

        #region AsNoTracking

        [Test]
        public void AsNoTracking_DefaultToTrue()
        {
            var mockSpecification = new Mock<LinqSpecification<string>>();

            Assert.IsTrue(mockSpecification.Object.IsTracked);
        }

        [Test]
        public void AsNoTracking_SetsIsTrackedFalse()
        {
            var mockSpecification = new Mock<LinqSpecification<string>>();

            mockSpecification.Object.AsNoTracking();

            Assert.IsFalse(mockSpecification.Object.IsTracked);
        }

        #endregion AsNoTracking
    }
}