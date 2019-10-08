using System;
using DevGate.Data.Contexts;
using DevGate.Data.Extensions.Services;
using Microsoft.EntityFrameworkCore;

namespace DevGate.Data.Migrations
{
	/// <summary>
	/// Migratable object
	/// </summary>
	public abstract class Migratable<TContext> : IMigratable where TContext : DbContext, IDbContext
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Required to allow Migratble to be typed")]
		private readonly TContext _context;

		/// <summary>
		/// Migrate the object using the <see cref="IServiceProvider"/>
		/// </summary>
		/// <param name="services"></param>
		public void Migrate(IServiceProvider services)
		{
			services.Migrate<TContext>();
		}

		/// <summary>
		/// Seeds the object using the <see cref="IServiceProvider"/>
		/// </summary>
		/// <param name="services"></param>
		public void SeedDatabase(IServiceProvider services)
		{
			services.Seed<TContext>();
		}
	}
}