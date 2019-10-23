using DevGate.Data.Extensions.Tests;
using DevGate.Data.Specifications;
using DevGate.Data.Tests.TestClasses;
using Moq;
using NUnit.Framework;

namespace DevGate.Data.Tests.SpecificationTests
{
	[TestFixture]
	public class AndSpecificationTests
	{
		[Test]
		public void Evaluate_GivenSingleMatchingSpecifications_ReturnsProductAsFalse()
		{
			// Arrange
			var mockLeftSpecification = new Mock<Specification<TestEntity>>();
			mockLeftSpecification.Setup(s => s.Evaluate())
				.Returns((e) => e.Text.StartsWith("A"));
			var mockRightSpecification = new Mock<Specification<TestEntity>>();
			mockRightSpecification.Setup(s => s.Evaluate())
				.Returns((e) => e.Text.EndsWith("B"));

			var andSpecification = new AndSpecification<TestEntity>(mockLeftSpecification.Object, mockRightSpecification.Object);

			// Act
			var result = andSpecification.Satisfies(new TestEntity { Text = "AA" });

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void Evaluate_GivenTwoMatchingSpecifications_ReturnsProductAsTrue()
		{
			// Arrange
			var mockLeftSpecification = new Mock<Specification<TestEntity>>();
			mockLeftSpecification.Setup(s => s.Evaluate())
				.Returns((e) => e.Text.StartsWith("A"));
			var mockRightSpecification = new Mock<Specification<TestEntity>>();
			mockRightSpecification.Setup(s => s.Evaluate())
				.Returns((e) => e.Text.EndsWith("B"));

			var andSpecification = new AndSpecification<TestEntity>(mockLeftSpecification.Object, mockRightSpecification.Object);

			// Act
			var result = andSpecification.Satisfies(new TestEntity { Text = "AB" });

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void Evaluate_GivenTwoNonMatchingSpecifications_ReturnsProductAsFalse()
		{
			// Arrange
			var mockLeftSpecification = new Mock<Specification<TestEntity>>();
			mockLeftSpecification.Setup(s => s.Evaluate())
				.Returns((e) => e.Text.StartsWith("B"));
			var mockRightSpecification = new Mock<Specification<TestEntity>>();
			mockRightSpecification.Setup(s => s.Evaluate())
				.Returns((e) => e.Text.EndsWith("B"));

			var andSpecification = new AndSpecification<TestEntity>(mockLeftSpecification.Object, mockRightSpecification.Object);

			// Act
			var result = andSpecification.Satisfies(new TestEntity { Text = "AA" });

			// Assert
			Assert.IsFalse(result);
		}
	}
}