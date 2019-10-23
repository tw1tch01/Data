using DevGate.Domain.Entities;
using DevGate.Data.Specifications;

namespace DevGate.Data.Extensions.Tests
{
	/// <summary>
	/// Testing extension methods for <see cref="Specification{TEntity}"/>
	/// </summary>
	public static class SpecificationTestExtensions
	{
		/// <summary>
		/// Determines if the specification satisfies the entity
		/// </summary>
		/// <param name="specification">Specification instance</param>
		/// <param name="entity">Entity instance</param>
		/// <returns>Whether entity is satisfied by Specifcation</returns>
		public static bool Satifies<TEntity>(this Specification<TEntity> specification, TEntity entity) where TEntity : BaseEntity
		{
			return specification.Evaluate().Compile()(entity);
		}
	}
}
