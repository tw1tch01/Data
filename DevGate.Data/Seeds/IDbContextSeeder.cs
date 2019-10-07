using System.Collections.Generic;
using DevGate.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevGate.Data.Seeds
{
	/// <summary>
	/// Seeder for <see cref="DbContext"/>
	/// </summary>
	/// <typeparam name="TContext">Context instance</typeparam>
	public interface IDbContextSeeder<in TContext> : IDbContextSeeder where TContext : DbContext, IDbContext
	{
		/// <summary>
		/// Seeds into specified context
		/// </summary>
		/// <param name="context"></param>
		void Seed(TContext context);
	}

	/// <summary>
	/// Seeder for <see cref="DbContext"/>
	/// </summary>
	public interface IDbContextSeeder
	{
		/// <summary>
		/// Gets the number of executions on the seeder in one process.
		/// </summary>
		IReadOnlyDictionary<string, int> Executions { get; }
	}

	/// <summary>
	/// Seeder options
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public interface IDbContextSeederOptions<TContext> where TContext : DbContext, IDbContext
	{
		/// <summary>
		/// Logger to be used by the context seeder
		/// </summary>
		ILogger Logger { get; }
	}
}