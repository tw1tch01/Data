using System;
using Data.Contexts;
using Data.Repositories;

namespace Data.IntegrationTests
{
    internal class MemoryDbInstance : IDisposable
    {
        public MemoryDbInstance()
        {
            Context = MemoryContextFactory.Create();
            Repository = RepositoryFactory.Create(Context);
        }

        public MemoryContext Context { get; private set; }
        public EntityRepository<IAuditedContext> Repository { get; private set; }

        public void Dispose()
        {
            MemoryContextFactory.Destroy(Context);
        }
    }
}