using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevGate.Domain.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace DevGate.Data.Specifications
{
	/// <summary>
	/// Entity-based specifications designed after the Specification and Repository pattern, to be used with <see cref="Microsoft.EntityFrameworkCore"/>
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public abstract class Specification<TEntity> where TEntity : BaseEntity
	{
		#region Fields

		private readonly List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> _modifiers = new List<Func<IQueryable<TEntity>, IQueryable<TEntity>>>();
		private readonly List<string> _tags = new List<string>();

		#endregion Fields

		#region Properties

		/// <summary>
		/// Indicates whether the entity is traced in the context
		/// </summary>
		public bool AsNoTracking { get; private set; }

		/// <summary>
		/// Indicates whether Distinct modifier has been applied
		/// </summary>
		public bool IsDistinct { get; private set; }

		/// <summary>
		/// Returns current modifiers specified on the specification
		/// </summary>
		public IReadOnlyCollection<Func<IQueryable<TEntity>, IQueryable<TEntity>>> Modifiers => _modifiers;

		/// <summary>
		/// Tag for the specification
		/// </summary>
		public virtual string Tag { get; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Add tag to specification
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public Specification<TEntity> AddTag(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag)) return this;

			_tags.Add(tag);
			return this;
		}

		/// <summary>
		/// Returns product of the two <see cref="Specification{TEntity}"/>
		/// </summary>
		/// <param name="specification"></param>
		/// <returns></returns>
		public Specification<TEntity> And(Specification<TEntity> specification)
		{
			var newSpecification = new AndSpecification<TEntity>(this, specification)
			{
				AsNoTracking = AsNoTracking || specification.AsNoTracking
			};

			if (string.IsNullOrWhiteSpace(Tag)) specification._tags.Add(Tag);

			if (string.IsNullOrWhiteSpace(specification.Tag)) specification._tags.Add(Tag);

			return newSpecification;
		}

		/// <summary>
		/// Adds <see cref="IQueryable{TEntity}"/> modifier to the specification that returns a distinct collection
		/// </summary>
		/// <returns></returns>
		public Specification<TEntity> Distinct()
		{
			if (IsDistinct) return this;

			_modifiers.Add((IQueryable<TEntity> q) => q.Distinct());
			IsDistinct = true;
			return this;
		}

		/// <summary>
		/// Adds <see cref="IQueryable{TEntity}"/> modifier to the specification that returns a distinct collection based on the property
		/// </summary>
		/// <returns></returns>
		public Specification<TEntity> DistinctBy<TProperty>(Expression<Func<TEntity, TProperty>> property)
		{
			if (IsDistinct) return this;

			_modifiers.Add((IQueryable<TEntity> q) => q.GroupBy(property).Select(x => x.FirstOrDefault()));
			IsDistinct = true;
			return this;
		}

		/// <summary>
		/// Filter the provided <see cref="IQueryable{TEntity}"/> with current specifications
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public virtual IQueryable<TEntity> Filter(IQueryable<TEntity> query)
		{
			var filter = ToExpression();

			if (filter == null) return query;

			return query.Where(filter.Expand());
		}

		/// <summary>
		/// Indicates whether the specification has the tag
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool HasTag(string tag)
		{
			return _tags.Union(new[] { Tag }).Contains(tag);
		}

		/// <summary>
		/// Adds <see cref="IQueryable{TEntity}"/> modifier to the specification to include the property in the query result
		/// </summary>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		public Specification<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> property)
		{
			if (property == null) throw new ArgumentNullException(nameof(property));

			_modifiers.Add((q) => q.Include(property));
			return this;
		}

		/// <summary>
		/// Modify the provided <see cref="IQueryable{TEntity}"/> with current modifiers
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public virtual IQueryable<TEntity> Modify(IQueryable<TEntity> query)
		{
			Modifiers.ToList().ForEach(modifier => query = modifier(query));
			return query;
		}

		/// <summary>
		/// Returns combination of the two <see cref="Specification{TEntity}" />
		/// </summary>
		/// <param name="specification"></param>
		/// <returns></returns>
		public Specification<TEntity> Or(Specification<TEntity> specification)
		{
			var newSpecification = new OrSpecification<TEntity>(this, specification)
			{
				AsNoTracking = AsNoTracking || specification.AsNoTracking
			};

			if (string.IsNullOrWhiteSpace(Tag)) specification._tags.Add(Tag);

			if (string.IsNullOrWhiteSpace(specification.Tag)) specification._tags.Add(Tag);

			return newSpecification;
		}

		/// <summary>
		/// Adds <see cref="IQueryable{TEntity}"/> modifier to the specification that sorts the query results in ascending order
		/// </summary>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		public Specification<TEntity> OrderBy<TProperty>(Expression<Func<TEntity, TProperty>> property)
		{
			if (property == null) throw new ArgumentNullException(nameof(property));

			if (_modifiers.LastOrDefault(m => m is IOrderedQueryable<TEntity>) != null)
				_modifiers.Add((q) => q.OrderBy(property));
			else
				_modifiers.Add((q) => (q as IOrderedQueryable<TEntity>).ThenBy(property));

			return this;
		}

		/// <summary>
		/// Adds <see cref="IQueryable{TEntity}"/> modifier to the specification that sorts the query results in descending order
		/// </summary>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public Specification<TEntity> OrderByDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			if (_modifiers.LastOrDefault(m => m is IOrderedQueryable<TEntity>) != null)
				_modifiers.Add((q) => q.OrderByDescending(expression));
			else
				_modifiers.Add((q) => (q as IOrderedQueryable<TEntity>).ThenByDescending(expression));

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public abstract Expression<Func<TEntity, bool>> ToExpression();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Specification<TEntity> WithNoTracking()
		{
			AsNoTracking = true;
			return this;
		}

		#endregion Methods
	}
}