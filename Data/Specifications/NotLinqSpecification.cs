using System;
using System.Linq.Expressions;
using Data.Extensions;

namespace Data.Specifications
{
    public class NotLinqSpecification<TType> : LinqSpecification<TType> where TType : class
    {
        private readonly LinqSpecification<TType> _spec;

        public NotLinqSpecification(LinqSpecification<TType> spec)
        {
            _spec = spec ?? throw new ArgumentNullException(nameof(spec));
            IsTracked = spec.IsTracked;
            IsDistinct = spec.IsDistinct;
            Modifiers = spec.Modifiers;
        }

        public override Expression<Func<TType, bool>> AsExpression()
        {
            return _spec.AsExpression().Not();
        }

        public override bool IsSatisfiedBy(TType candidate)
        {
            return !_spec.IsSatisfiedBy(candidate);
        }
    }
}