using System;
using System.Linq.Expressions;
using DevGate.Domain.Entities;
using LinqKit;

namespace DevGate.Data.Specifications
{
	/// <summary>
	/// Combination of two (2) <see cref="Specification{TEntity}"/>
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class OrSpecification<TEntity> : Specification<TEntity> where TEntity : BaseEntity
	{
		private readonly Specification<TEntity> _left;
		private readonly Specification<TEntity> _right;

		/// <summary>
		/// Initializes a new instance of <see cref="OrSpecification{TEntity}"/>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public OrSpecification(Specification<TEntity> left, Specification<TEntity> right)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));
		}

		/// <summary>
		/// Overrides base <see cref="Specification{TEntity}.ToExpression"/> to return OR combination of two <see cref="Specification{TEntity}"/>
		/// </summary>
		/// <returns></returns>
		public override Expression<Func<TEntity, bool>> ToExpression()
		{
			return _left.ToExpression().Or(_right.ToExpression());
		}
	}
}