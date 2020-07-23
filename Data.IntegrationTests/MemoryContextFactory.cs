namespace Data.IntegrationTests
{
    internal static class MemoryContextFactory
    {
        public static MemoryContext Create()
        {
            var context = new MemoryContext();
            context.Database.EnsureCreated();
            context.Seed();

            return context;
        }

        public static void Destroy(MemoryContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}