﻿using System;
using Data.Specifications;
using Moq;
using NUnit.Framework;

namespace Data.UnitTests.Specifications
{
    [TestFixture]
    public class AndSpecificationTests
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

            var andSpec = mockLeftSpec.Object & mockRightSpec.Object;

            Assert.IsFalse(andSpec.IsSatisfiedBy(It.IsAny<string>()));
        }

        [Test]
        public void IsSatisfiedBy_WhenLeftSpecIsSatisfiedButRightSpecIsNotSatisfied_ReturnsFalse()
        {
            var mockLeftSpec = new Mock<Specification<string>>();
            var mockRightSpec = new Mock<Specification<string>>();

            mockLeftSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(true);
            mockRightSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(false);

            var andSpec = mockLeftSpec.Object & mockRightSpec.Object;

            Assert.IsFalse(andSpec.IsSatisfiedBy(It.IsAny<string>()));
        }

        [Test]
        public void IsSatisfiedBy_WhenLeftSpecIsNotSatisfiedButRightSpecIsSatisfied_ReturnsFalse()
        {
            var mockLeftSpec = new Mock<Specification<string>>();
            var mockRightSpec = new Mock<Specification<string>>();

            mockLeftSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(false);
            mockRightSpec.Setup(s => s.IsSatisfiedBy(It.IsAny<string>()))
                .Returns(true);

            var andSpec = mockLeftSpec.Object & mockRightSpec.Object;

            Assert.IsFalse(andSpec.IsSatisfiedBy(It.IsAny<string>()));
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

            var andSpec = mockLeftSpec.Object & mockRightSpec.Object;

            Assert.IsTrue(andSpec.IsSatisfiedBy(It.IsAny<string>()));
        }

        [Test]
        public void WhenLeftSpecIsNull_ThrowsArgumentNullExceptionForLeftSpec()
        {
            Specification<string> leftSpec = null;
            var mockRightSpec = new Mock<Specification<string>>();

            var exception = Assert.Catch<ArgumentNullException>(() =>
         {
             var andSpec = leftSpec & mockRightSpec.Object;
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
             var _ = mockLeftSpec.Object & rightSpec;
         });
            Assert.AreEqual("Value cannot be null. (Parameter 'rightSpec')", exception.Message);
        }
    }
}