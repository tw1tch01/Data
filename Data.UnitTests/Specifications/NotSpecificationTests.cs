using System;
using Data.Specifications;
using Moq;
using NUnit.Framework;

namespace Data.UnitTests.Specifications
{
    [TestFixture]
    public class NotSpecificationTests
    {
        [Test]
        public void IsSatisfiedBy_WhenSpecIsNotSatisfied_ReturnsTrue()
        {
            var mockSpec = new Mock<Specification<string>>();
            mockSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(false);

            var notSpec = !mockSpec.Object;

            Assert.IsTrue(notSpec.IsSatisfiedBy(It.IsAny<string>()));
        }

        [Test]
        public void IsSatisfiedBy_WhenSpecIsSatisfied_ReturnsFalse()
        {
            var mockSpec = new Mock<Specification<string>>();
            mockSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(true);

            var notSpec = !mockSpec.Object;

            Assert.IsFalse(notSpec.IsSatisfiedBy(It.IsAny<string>()));
        }

        [Test]
        public void WhenLeftSpecIsNull_ThrowsArgumentNullExceptionForLeftSpec()
        {
            Specification<string> spec = null;

            var exception = Assert.Catch<ArgumentNullException>(() =>
            {
                var _ = !spec;
            });
            Assert.AreEqual("Value cannot be null. (Parameter 'spec')", exception.Message);
        }
    }
}