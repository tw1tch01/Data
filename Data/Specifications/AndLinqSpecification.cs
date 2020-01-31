using System;
using System.Linq;
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
            IsTracked = leftSpec.IsTracked || rightSpec.IsTracked;
            IsDistinct = leftSpec.IsDistinct || rightSpec.IsDistinct;
            Modifiers = leftSpec.Modifiers.Union(rightSpec.Modifiers).ToList();
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