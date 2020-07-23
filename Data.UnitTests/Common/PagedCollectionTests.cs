using System.Collections.Generic;
using Data.Common;
using Moq;
using NUnit.Framework;

namespace Data.UnitTests.Common
{
    [TestFixture]
    public class PagedCollectionTests
    {
        [TestCase(50, 100, 50, 2)]
        [TestCase(200, 200, 200, 1)]
        [TestCase(10, 150, 1, 15)]
        public void PageCount_ArePropertiesAreSet(int pageSize, int totalRecords, int itemCount, int pageCount)
        {
            var page = 0;
            var items = new Mock<ICollection<string>>();
            items.Setup(a => a.Count).Returns(itemCount);

            var result = new PagedCollection<string>(page, pageSize, totalRecords, items.Object);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(page, result.Page);
                Assert.AreEqual(pageSize, result.PageSize);
                Assert.AreEqual(pageCount, result.PageCount);
                Assert.AreEqual(totalRecords, result.TotalRecords);
                Assert.AreEqual(itemCount, result.Items.Count);
            });
        }

        [TestCase(50, 100, 50, 2)]
        [TestCase(200, 200, 200, 1)]
        [TestCase(10, 150, 1, 15)]
        [TestCase(200, 1, 1, 1)]
        public void PageCount_IsCalculatedCorrectly(int pageSize, int totalRecords, int itemCount, int expected)
        {
            var page = 0;
            var items = new Mock<ICollection<string>>();
            items.Setup(a => a.Count).Returns(itemCount);

            var result = new PagedCollection<string>(page, pageSize, totalRecords, items.Object);

            Assert.AreEqual(expected, result.PageCount);
        }
    }
}