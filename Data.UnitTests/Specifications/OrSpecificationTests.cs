using System;
using Data.Specifications;
using Moq;
using NUnit.Framework;

namespace Data.UnitTests.Specifications
{
    [TestFixture]
    public class OrSpecificationTests
    {
        [Test]
        public void IsSatisfiedBy_WhenBothLeftAndRightSpecAreNotSatisfied_ReturnsFalse()
        {
            var mockLeftSpec = new Mock<Specification<string>>();
            var mockRightSpec = new Mock<Specification<string>>();

            mockLeftSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(false);
            mockRightSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(false);

            var orSpec = mockLeftSpec.Object | mockRightSpec.Object;

            Assert.IsFalse(orSpec.IsSatisfiedBy(It.IsAny<string>()));
        }

        [Test]
        public void IsSatisfiedBy_WhenLeftSpecIsSatisfiedButRightSpecIsNotSatisfied_ReturnsTrue()
        {
            var mockLeftSpec = new Mock<Specification<string>>();
            var mockRightSpec = new Mock<Specification<string>>();

            mockLeftSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(true);
            mockRightSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(false);

            var orSpec = mockLeftSpec.Object | mockRightSpec.Object;

            Assert.IsTrue(orSpec.IsSatisfiedBy(It.IsAny<string>()));
        }

        [Test]
        public void IsSatisfiedBy_WhenLeftSpecIsNotSatisfiedButRightSpecIsSatisfied_ReturnsTrue()
        {
            var mockLeftSpec = new Mock<Specification<string>>();
            var mockRightSpec = new Mock<Specification<string>>();

            mockLeftSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(false);
            mockRightSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(true);

            var orSpec = mockLeftSpec.Object | mockRightSpec.Object;

            Assert.IsTrue(orSpec.IsSatisfiedBy(It.IsAny<string>()));
        }

        [Test]
        public void IsSatisfiedBy_WhenBothLeftAndRightSpecAreSatisfied_ReturnsTrue()
        {
            var mockLeftSpec = new Mock<Specification<string>>();
            var mockRightSpec = new Mock<Specification<string>>();

            mockLeftSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(true);
            mockRightSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(true);

            var orSpec = mockLeftSpec.Object | mockRightSpec.Object;

            Assert.IsTrue(orSpec.IsSatisfiedBy(It.IsAny<string>()));
        }

        [Test]
        public void WhenLeftSpecIsNull_ThrowsArgumentNullExceptionForLeftSpec()
        {
            Specification<string> leftSpec = null;
            var mockRightSpec = new Mock<Specification<string>>();

            var exception = Assert.Catch<ArgumentNullException>(() =>
            {
                var _ = leftSpec | mockRightSpec.Object;
            });
            Assert.AreEqual("Value cannot be null. (Parameter 'leftSpec')", exception.Message);
        }

        [Test]
        public void WhenRightSpecIsNull_ThrowsArgumentNullExceptionForRightSpec()
        {
            var mockLeftSpec = new Mock<Specification<string>>();
            Specification<string> rightSpec = null;

            var exception = Assert.Catch<ArgumentNullException>(() =>
            {
                var _ = mockLeftSpec.Object | rightSpec;
            });
            Assert.AreEqual("Value cannot be null. (Parameter 'rightSpec')", exception.Message);
        }
    }
}