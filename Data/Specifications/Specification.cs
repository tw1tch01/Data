namespace Data.Specifications
{
	public abstract class Specification<TType> where TType : class
	{
		public static Specification<TType> operator &(Specification<TType> leftSpec, Specification<TType> rightSpec)
		{
			return new AndSpecification<TType>(leftSpec, rightSpec);
		}

		public static Specification<TType> operator |(Specification<TType> leftSpec, Specification<TType> rightSpec)
		{
			return new OrSpecification<TType>(leftSpec, rightSpec);
		}

		public static Specification<TType> operator !(Specification<TType> spec)
		{
			return new NotSpecification<TType>(spec);
		}

		public abstract bool IsSatisfiedBy(TType candidate);
	}
}