using System;
using DevGate.Data.Contexts;
using DevGate.Data.Extensions.Services;
using Microsoft.EntityFrameworkCore;

namespace DevGate.Data.Migrations
{
	public class Migratable<TContext> : IMigratable where TContext : DbContext, IDbContext
	{
		private readonly TContext _context;

		public void Migrate(IServiceProvider services)
		{
			services.Migrate<TContext>();
		}

		public void SeedDatabase(IServiceProvider services)
		{
			services.Seed<TContext>();
		}
	}
}