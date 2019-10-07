using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DevGate.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevGate.Data.Seeds
{
	public class SqlSeeder<TContext> : IDbContextSeeder<TContext> where TContext : DbContext, IDbContext
	{
		#region Fields

		private readonly Assembly _assembly;
		private readonly Dictionary<string, int> _executions = new Dictionary<string, int>();
		private readonly ICollection<string> _fileSuffixes;
		private readonly ILogger _logger;
		private readonly string _scriptSeparator;

		#endregion Fields

		#region Constructor

		public SqlSeeder(SqlSeederOptions<TContext> options)
		{
			_assembly = options.Assembly;
			_fileSuffixes = options.FileSuffixes;
			_logger = options.Logger;
			_scriptSeparator = options.ScriptSeparator;
		}

		#endregion Constructor

		public IReadOnlyDictionary<string, int> Executions => _executions;

		public void Seed(TContext context)
		{
			_logger.LogInformation($"Retrieving scripts: [{string.Join(",", _fileSuffixes)}]");

			Array.ForEach(_fileSuffixes.ToArray(), (suffix) =>
			{
				var scripts = GetScripts(suffix);

				foreach (var script in scripts)
				{

				}
			});
		}

		internal IDictionary<string, string> GetScripts(string suffix)
		{
			if (string.IsNullOrWhiteSpace(suffix)) throw new ArgumentNullException(nameof(suffix));

			var scriptWithContent = new Dictionary<string, string>();

			_assembly.GetManifestResourceNames().Where(mr => mr.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)).ToList().ForEach(manifest =>
			{
				using (var reader = new StreamReader(_assembly.GetManifestResourceStream(manifest)))
				{
					SplitSqlCode(reader.ReadToEnd()).ToList().ForEach(commands => scriptWithContent.Add(manifest, commands));
				}
			});

			return scriptWithContent;
		}

		internal ICollection<string> SplitSqlCode(string sql)
		{
			if (string.IsNullOrWhiteSpace(_scriptSeparator)) return new List<string> { sql };

			return Regex.Split(sql, $"^\\s*{_scriptSeparator}\\s*$", RegexOptions.Multiline)
						.Where(b => b.Trim().Length > 0)
						.Select(b => b.Trim())
						.ToList();
		}
	}

	public class SqlSeederOptions<TContext> : IDbContextSeederOptions<TContext> where TContext : DbContext, IDbContext
	{
		public Assembly Assembly { get; set; }

		public ILogger Logger { get; set; }

		public string ScriptSeparator { get; set; } = Constants.Separator;

		public ICollection<string> FileSuffixes { get; set; } = DefaultSqlFileSuffixes();

		private static ICollection<string> DefaultSqlFileSuffixes()
		{
			return new List<string>
			{
				".synonym.sql",
				".view.sql",
				".procedure.sql",
				".function.sql",
				".ddltrigger.sql",
			};
		}

		private class Constants
		{
			public const string Separator = "GO";
		}
	}
}