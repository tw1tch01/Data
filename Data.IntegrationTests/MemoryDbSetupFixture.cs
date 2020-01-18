using NUnit.Framework;

namespace Data.IntegrationTests
{
    [SetUpFixture]
    internal class MemoryDbSetupFixture
    {
        protected MemoryDbInstance _memoryDb;

        [OneTimeSetUp]
        public void SetupFixtureInit()
        {
            _memoryDb = new MemoryDbInstance();
        }

        [OneTimeTearDown]
        public void SetupFixtureCleanup()
        {
            _memoryDb.Dispose();
        }
    }
}