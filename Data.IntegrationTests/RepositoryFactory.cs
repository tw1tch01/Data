using Data.Contexts;
using Data.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Data.IntegrationTests
{
    internal class RepositoryFactory
    {
        public static EntityRepository<IAuditedContext> Create(IAuditedContext context)
        {
            return new EntityRepository<IAuditedContext>(context, new Mock<ILogger<IAuditedContext>>().Object);
        }
    }
}