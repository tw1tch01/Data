using System.Threading;
using AutoFixture;
using DevGate.Data.Contexts;
using DevGate.Data.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DevGate.Data.Tests.RepositoryTests
{
	[TestFixture]
	public partial class EntityRepositoryTests
	{
		private readonly Fixture _fixture = new Fixture();

		private EntityRepository<TContext> CreateRepo<TContext>(Mock<TContext> mockContext = null, Mock<ILogger<TContext>> mockLogger = null) where TContext : class, IDbContext
		{
			mockContext ??= new Mock<TContext>();
			mockLogger ??= new Mock<ILogger<TContext>>();

			return new EntityRepository<TContext>(mockContext.Object, mockLogger.Object);
		}

		private void NotSaved<TContext>(Mock<TContext> mockDbContext) where TContext : class, IDbContext
		{
			mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never, "Context should never saved.");
		}
	}
}