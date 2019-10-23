using System;
using System.Linq.Expressions;
using DevGate.Domain.Entities;
using LinqKit;

namespace DevGate.Data.Specifications
{
	/// <summary>
	/// Product of two (2) <see cref="Specification{TEntity}"/>
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class AndSpecification<TEntity> : Specification<TEntity> where TEntity : BaseEntity
	{
		private readonly Specification<TEntity> _left;
		private readonly Specification<TEntity> _right;

		/// <summary>
		/// Initializes a new instance of <see cref="AndSpecification{TEntity}"/>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public AndSpecification(Specification<TEntity> left, Specification<TEntity> right)
		{
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));
		}

		/// <summary>
		/// Overrides base <see cref="Specification{TEntity}.Evaluate"/> to return AND product of two specified <see cref="Specification{TEntity}"/>
		/// </summary>
		public override Expression<Func<TEntity, bool>> Evaluate()
		{
			return _left.Evaluate().And(_right.Evaluate());
		}
	}
}