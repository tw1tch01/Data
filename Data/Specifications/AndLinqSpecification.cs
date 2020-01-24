using System;
using System.Linq.Expressions;
using LinqKit;

namespace Data.Specifications
{
    public class AndLinqSpecification<TType> : LinqSpecification<TType> where TType : class
    {
        public readonly LinqSpecification<TType> _leftSpec;
        public readonly LinqSpecification<TType> _rightSpec;

        public AndLinqSpecification(LinqSpecification<TType> leftSpec, LinqSpecification<TType> rightSpec)
        {
            _leftSpec = leftSpec ?? throw new ArgumentNullException(nameof(leftSpec));
            _rightSpec = rightSpec ?? throw new ArgumentNullException(nameof(rightSpec));
        }

        public override Expression<Func<TType, bool>> AsExpression()
        {
            return _leftSpec.AsExpression().And(_rightSpec.AsExpression());
        }

        public override bool IsSatisfiedBy(TType candidate)
        {
            return _leftSpec.IsSatisfiedBy(candidate) && _rightSpec.IsSatisfiedBy(candidate);
        }
    }
}