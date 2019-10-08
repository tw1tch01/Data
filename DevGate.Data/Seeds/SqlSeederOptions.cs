using System.Collections.Generic;
using System.Reflection;
using DevGate.Data.Constants;
using DevGate.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevGate.Data.Seeds
{
	/// <summary>
	/// Configuration options for <see cref="SqlSeeder{TContext}"/> instance
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public class SqlSeederOptions<TContext> : IDbContextSeederOptions<TContext> where TContext : DbContext, IDbContext
	{
		/// <summary>
		/// Gets or sets the assembly to run the seeder on
		/// </summary>
		public Assembly Assembly { get; set; }

		/// <summary>
		/// Gets or sets the list of file suffixes to look for. If not set uses internal defaults.
		/// </summary>
		public ICollection<string> FileSuffixes { get; set; } = DefaultSqlFileSuffixes();

		/// <summary>
		/// Gets or sets the logger to use while seeding
		/// </summary>
		public ILogger Logger { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ScriptSeparator { get; set; } = Sql.Separator.Go;

		private static ICollection<string> DefaultSqlFileSuffixes()
		{
			return new List<string>
			{
				Sql.Suffixes.Functions,
				Sql.Suffixes.Procudures,
				Sql.Suffixes.Synonyms,
				Sql.Suffixes.Triggers,
				Sql.Suffixes.Views,
			};
		}
	}
}