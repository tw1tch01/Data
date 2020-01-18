using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Data.Specifications
{
	public abstract class LinqSpecification<TType> : Specification<TType> where TType : class
	{
		#region Properties

		public bool IsDistinct { get; private set; }
		public ICollection<Func<IQueryable<TType>, IQueryable<TType>>> Modifers { get; } = new List<Func<IQueryable<TType>, IQueryable<TType>>>();
		public bool IsTracked { get; private set; } = true;

		#endregion Properties

		#region Methods

		public abstract Expression<Func<TType, bool>> AsExpression();

		public override bool IsSatisfiedBy(TType candidate)
		{
			return AsExpression().Compile()(candidate);
		}

		public LinqSpecification<TType> AddQuery(Func<IQueryable<TType>, IQueryable<TType>> query)
		{
			Modifers.Add(query);
			return this;
		}

		public LinqSpecification<TType> Distinct()
		{
			if (IsDistinct) return this;

			Modifers.Add((IQueryable<TType> q) => q.Distinct());
			IsDistinct = true;
			return this;
		}

		public LinqSpecification<TType> DistinctBy<TProperty>(Expression<Func<TType, TProperty>> property)
		{
			if (IsDistinct) return this;

			Modifers.Add((IQueryable<TType> q) => q.GroupBy(property).Select(x => x.FirstOrDefault()));
			IsDistinct = true;
			return this;
		}

		public LinqSpecification<TType> Include<TProperty>(Expression<Func<TType, TProperty>> property)
		{
			if (property == null) throw new ArgumentNullException(nameof(property));

			Modifers.Add((q) => q.Include(property));
			return this;
		}

		public LinqSpecification<TType> OrderBy<TProperty>(Expression<Func<TType, TProperty>> property)
		{
			if (property == null) throw new ArgumentNullException(nameof(property));

			var isOrderBy = Modifers.LastOrDefault(m => m is IOrderedQueryable<TType>) != null;
			if (!isOrderBy)
			{
				IOrderedQueryable<TType> orderByQuery(IQueryable<TType> q) => q.OrderBy(property);
				Modifers.Add(orderByQuery);
			}
			else
			{
				IOrderedQueryable<TType> thenByQuery(IQueryable<TType> q) => ((IOrderedQueryable<TType>)q).ThenBy(property);
				Modifers.Add(thenByQuery);
			}

			return this;
		}

		public LinqSpecification<TType> OrderByDescending<TProperty>(Expression<Func<TType, TProperty>> property)
		{
			if (property == null) throw new ArgumentNullException(nameof(property));

			var isOrderBy = Modifers.LastOrDefault(m => m is IOrderedQueryable<TType>) != null;
			if (!isOrderBy)
			{
				IOrderedQueryable<TType> orderByQuery(IQueryable<TType> q) => q.OrderByDescending(property);
				Modifers.Add(orderByQuery);
			}
			else
			{
				IOrderedQueryable<TType> thenByQuery(IQueryable<TType> q) => ((IOrderedQueryable<TType>)q).ThenByDescending(property);
				Modifers.Add(thenByQuery);
			}
			return this;
		}

		public LinqSpecification<TType> AsNoTracking()
		{
			IsTracked = false;
			return this;
		}

		public virtual IQueryable<TType> ApplyFilters(IQueryable<TType> query)
		{
			var filter = AsExpression();

			if (filter == null) return query;

			return query.Where(filter.Expand());
		}

		public virtual IQueryable<TType> ApplyModifiers(IQueryable<TType> query)
		{
			Modifers.ToList().ForEach(m => query = m(query));
			return query;
		}

		#endregion Methods
	}
}