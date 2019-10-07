using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevGate.Data.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace DevGate.Data.Specifications
{
	public abstract class Specification<TEntity> where TEntity : BaseEntity
	{
		#region Fields

		private readonly List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> _modifiers = new List<Func<IQueryable<TEntity>, IQueryable<TEntity>>>();
		private readonly List<string> _tags = new List<string>();

		#endregion Fields

		#region Properties

		public bool AsNoTracking { get; set; }
		public bool IsDistinct { get; private set; }

		public IReadOnlyCollection<Func<IQueryable<TEntity>, IQueryable<TEntity>>> Modifiers => _modifiers;

		public virtual string Tag { get; }

		#endregion Properties

		#region Methods

		public Specification<TEntity> AddTag(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag)) return this;

			_tags.Add(tag);
			return this;
		}

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

		public virtual IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query)
		{
			var filter = ToExpression();

			if (filter == null) return query;

			return query.Where(filter.Expand());
		}

		public virtual IQueryable<TEntity> ApplyModifiers(IQueryable<TEntity> query)
		{
			Modifiers.ToList().ForEach(modifier => query = modifier(query));
			return query;
		}

		public Specification<TEntity> Distinct()
		{
			_modifiers.Add((IQueryable<TEntity> q) => q.Distinct());
			IsDistinct = true;
			return this;
		}

		public bool HasTag(string tag)
		{
			return _tags.Union(new[] { Tag }).Contains(tag);
		}

		public Specification<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			_modifiers.Add((q) => q.Include(expression));
			return this;
		}

		public Specification<TEntity> OrderBy<TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			if (_modifiers.LastOrDefault(m => m is IOrderedQueryable<TEntity>) != null)
				_modifiers.Add((q) => q.OrderBy(expression));
			else
				_modifiers.Add((q) => (q as IOrderedQueryable<TEntity>).ThenBy(expression));

			return this;
		}

		public Specification<TEntity> OrderByDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			if (_modifiers.LastOrDefault(m => m is IOrderedQueryable<TEntity>) != null)
				_modifiers.Add((q) => q.OrderByDescending(expression));
			else
				_modifiers.Add((q) => (q as IOrderedQueryable<TEntity>).ThenByDescending(expression));

			return this;
		}

		public bool Satifies(TEntity entity)
		{
			Func<TEntity, bool> function = ToExpression().Compile();
			return function(entity);
		}

		public abstract Expression<Func<TEntity, bool>> ToExpression();

		public Specification<TEntity> WithNoTracking()
		{
			AsNoTracking = true;
			return this;
		}

		#endregion Methods
	}
}