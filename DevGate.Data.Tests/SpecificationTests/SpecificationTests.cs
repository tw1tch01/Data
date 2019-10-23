using DevGate.Data.Extensions.Tests;
using DevGate.Data.Specifications;
using DevGate.Domain.Entities;
using Moq;
using NUnit.Framework;

namespace DevGate.Data.Tests.SpecificationTests
{
	[TestFixture]
	public class SpecificationTests
	{
		[Test]
		public void Add_GivenSpecification_ReturnsAddSpecificationType()
		{
			// Arrange
			var mockSpecifcation1 = new Mock<Specification<BaseEntity>>();
			var mockSpecifcation2 = new Mock<Specification<BaseEntity>>();

			// Act
			var newSpec = mockSpecifcation1.Object.And(mockSpecifcation2.Object);

			// Assert
			Assert.IsInstanceOf(typeof(AndSpecification<BaseEntity>), newSpec);
		}

		[Test]
		public void Distinct_ShouldAddExpressionToQueryModifiers()
		{
			// Arrange
			var mockSpecifcation = new Mock<Specification<BaseEntity>>();

			// Act
			mockSpecifcation.Object.Distinct();

			// Assert
			Assert.AreEqual(1, mockSpecifcation.Object.Modifiers.Count);
		}

		[Test]
		public void DistinctBy_ShouldAddExpressionToQueryModifiers()
		{
			// Arrange
			var mockSpecifcation = new Mock<Specification<BaseEntity>>();

			// Act
			mockSpecifcation.Object.DistinctBy(e => true);

			// Assert
			Assert.AreEqual(1, mockSpecifcation.Object.Modifiers.Count);
		}

		[Test]
		public void Evaluate_GivenMatchingExpression_ReturnsTrue()
		{
			// Arrange
			var mockSpecification = new Mock<Specification<BaseEntity>>();
			mockSpecification.Setup(s => s.Evaluate()).Returns((e) => e == null);
			BaseEntity entity = null;

			// Act
			var result = mockSpecification.Object.Satisfies(entity);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void Include_ShouldAddExpressionToQueryModifiers()
		{
			// Arrange
			var mockSpecifcation = new Mock<Specification<BaseEntity>>();

			// Act
			mockSpecifcation.Object.Include(e => e);

			// Assert
			Assert.AreEqual(1, mockSpecifcation.Object.Modifiers.Count);
		}

		[Test]
		public void Or_GivenSpecification_ReturnsOrSpecificationType()
		{
			// Arrange
			var mockSpecifcation1 = new Mock<Specification<BaseEntity>>();
			var mockSpecifcation2 = new Mock<Specification<BaseEntity>>();

			// Act
			var newSpec = mockSpecifcation1.Object.Or(mockSpecifcation2.Object);

			// Assert
			Assert.IsInstanceOf(typeof(OrSpecification<BaseEntity>), newSpec);
		}

		[Test]
		public void OrderBy_ShouldAddExpressionToQueryModifiers()
		{
			// Arrange
			var mockSpecifcation = new Mock<Specification<BaseEntity>>();

			// Act
			mockSpecifcation.Object.OrderBy(e => e);

			// Assert
			Assert.AreEqual(1, mockSpecifcation.Object.Modifiers.Count);
		}

		[Test]
		public void OrderByDescending_ShouldAddExpressionToQueryModifiers()
		{
			// Arrange
			var mockSpecifcation = new Mock<Specification<BaseEntity>>();

			// Act
			var newSpec = mockSpecifcation.Object.OrderByDescending(e => e);

			// Assert
			Assert.AreEqual(1, mockSpecifcation.Object.Modifiers.Count);
		}
	}
}