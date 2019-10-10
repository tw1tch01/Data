using System;
using System.Linq.Expressions;
using DevGate.Data.Specifications;

namespace DevGate.Data.Tests.TestClasses
{
	public class TestEntityFindById : Specification<TestEntity>
	{
		private readonly int _id;

		public TestEntityFindById(int id)
		{
			_id = id;
		}

		public override Expression<Func<TestEntity, bool>> ToExpression()
		{
			return entity => entity.Id == _id;
		}
	}
}