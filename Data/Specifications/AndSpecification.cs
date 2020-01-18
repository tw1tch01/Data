using System;

namespace Data.Specifications
{
	public class AndSpecification<TType> : Specification<TType> where TType : class
	{
		public readonly Specification<TType> _leftSpec;
		public readonly Specification<TType> _rightSpec;

		public AndSpecification(Specification<TType> leftSpec, Specification<TType> rightSpec)
		{
			_leftSpec = leftSpec ?? throw new ArgumentNullException(nameof(leftSpec));
			_rightSpec = rightSpec ?? throw new ArgumentNullException(nameof(rightSpec));
		}

		public override bool IsSatisfiedBy(TType candidate)
		{
			return _leftSpec.IsSatisfiedBy(candidate) && _rightSpec.IsSatisfiedBy(candidate);
		}
	}
}