using System;
using System.Linq.Expressions;
using DevGate.Data.Entities;
using LinqKit;

namespace DevGate.Data.Specifications
{
	public class AndSpecification<TEntity> : Specification<TEntity> where TEntity : BaseEntity
	{
		private readonly Specification<TEntity> _left;
		private readonly Specification<TEntity> _right;

		public AndSpecification(Specification<TEntity> left, Specification<TEntity> right)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));
		}

		public override Expression<Func<TEntity, bool>> ToExpression()
		{
			return _left.ToExpression().And(_right.ToExpression());
		}
	}
}