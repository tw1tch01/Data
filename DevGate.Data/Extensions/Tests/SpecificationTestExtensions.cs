using DevGate.Domain.Entities;
using DevGate.Data.Specifications;

namespace DevGate.Data.Extensions.Tests
{
	public static class SpecificationTestExtensions
	{
		/// <summary>
		/// Determines if the specification satisfies the entity
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static bool Satifies<TEntity>(this Specification<TEntity> specification, TEntity entity) where TEntity : BaseEntity
		{
			return specification.ToExpression().Compile()(entity);
		}
	}
}
