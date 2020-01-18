using System;

namespace Data.Specifications
{
	public class NotSpecification<TType> : Specification<TType> where TType : class
	{
		private readonly Specification<TType> _spec;

		public NotSpecification(Specification<TType> spec)
		{
			_spec = spec ?? throw new ArgumentNullException(nameof(spec));
		}

		public override bool IsSatisfiedBy(TType candidate)
		{
			return !_spec.IsSatisfiedBy(candidate);
		}
	}
}