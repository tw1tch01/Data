using System;
using System.Linq.Expressions;
using DevGate.Data.Entities;
using LinqKit;

namespace DevGate.Data.Specifications
{
	public class OrSpecification<TEntity> : Specification<TEntity> where TEntity : BaseEntity
	{
		private readonly Specification<TEntity> _left;
		private readonly Specification<TEntity> _right;

		public OrSpecification(Specification<TEntity> left, Specification<TEntity> right)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));
		}

		public override Expression<Func<TEntity, bool>> ToExpression()
		{
			return _left.ToExpression().Or(_right.ToExpression());
		}
	}
}