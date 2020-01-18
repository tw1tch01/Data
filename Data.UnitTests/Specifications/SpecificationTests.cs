using Data.Specifications;
using Moq;
using NUnit.Framework;

namespace Data.UnitTests.Specifications
{
    [TestFixture]
    public class SpecificationTests
    {
        [Test]
        public void AddOperator_ReturnsAndSpecification()
        {
            var mockLeftSpec = new Mock<Specification<string>>();
            var mockRightSpec = new Mock<Specification<string>>();

            var addSpec = mockLeftSpec.Object & mockRightSpec.Object;

            Assert.IsInstanceOf(typeof(AndSpecification<string>), addSpec);
        }

        [Test]
        public void OrOperator_ReturnsOrSpecification()
        {
            var mockLeftSpec = new Mock<Specification<string>>();
            var mockRightSpec = new Mock<Specification<string>>();

            var orSpec = mockLeftSpec.Object | mockRightSpec.Object;

            Assert.IsInstanceOf(typeof(OrSpecification<string>), orSpec);
        }

        [Test]
        public void NotOperator_ReturnsNotSpecification()
        {
            var mockSpec = new Mock<Specification<string>>();

            var notSpec = !mockSpec.Object;

            Assert.IsInstanceOf(typeof(NotSpecification<string>), notSpec);
        }

        [Test]
        public void IsSatisfiedBy_WhenCandidateIsSatisfied_ReturnsTrue()
        {
            var candidate = "";
            var mockSpec = new Mock<Specification<string>>();
            mockSpec.Setup(s => s.IsSatisfiedBy(candidate)).Returns(true);

            Assert.IsTrue(mockSpec.Object.IsSatisfiedBy(candidate));
        }

        [Test]
        public void IsSatisfiedBy_WhenCandidateIsNotSatisfied_ReturnsFalse()
        {
            var candidate = "";
            var mockSpec = new Mock<Specification<string>>();
            mockSpec.Setup(s => s.IsSatisfiedBy(candidate)).Returns(true);

            Assert.IsFalse(mockSpec.Object.IsSatisfiedBy(null));
        }
    }
}