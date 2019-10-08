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
	/// <summary>
	/// Sql seeder instance
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
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

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="options"></param>
		public SqlSeeder(SqlSeederOptions<TContext> options)
		{
			_assembly = options.Assembly;
			_fileSuffixes = options.FileSuffixes;
			_logger = options.Logger;
			_scriptSeparator = options.ScriptSeparator;
		}

		#endregion Constructor

		/// <summary>
		/// Gets the number of scripts executed during the migration
		/// </summary>
		public IReadOnlyDictionary<string, int> Executions => _executions;

		/// <summary>
		/// Seed the ob
		/// </summary>
		/// <param name="context"></param>
		public void Seed(TContext context)
		{
			_logger.LogInformation($"Retrieving scripts: [{string.Join(",", _fileSuffixes)}]");

			Array.ForEach(_fileSuffixes.ToArray(), (suffix) =>
			{
				var scripts = GetScripts(suffix);

				var i = 1;
				foreach (var script in scripts)
				{
					var scriptName = script.Key;

					_logger.LogInformation($"Executing part {i} of {scriptName}");
					if (_executions.TryGetValue(scriptName, out int count))
						_executions[scriptName] = ++count;
					else
						_executions.Add(scriptName, 1);

					context.Database.ExecuteSqlRaw(script.Value);
				}
			});
		}

		/// <summary>
		/// Returns all script contents from files that match the specified suffix
		/// </summary>
		/// <param name="suffix"></param>
		/// <returns></returns>
		internal IDictionary<string, string> GetScripts(string suffix)
		{
			if (string.IsNullOrWhiteSpace(suffix)) throw new ArgumentNullException(nameof(suffix));

			var scriptWithContent = new Dictionary<string, string>();

			_assembly.GetManifestResourceNames().Where(mr => mr.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)).ToList().ForEach(manifest =>
			{
				using var reader = new StreamReader(_assembly.GetManifestResourceStream(manifest));
				SplitSqlCode(reader.ReadToEnd()).ToList().ForEach(commands => scriptWithContent.Add(manifest, commands));
			});

			return scriptWithContent;
		}

		/// <summary>
		/// Split SQL cod
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		internal ICollection<string> SplitSqlCode(string sql)
		{
			if (string.IsNullOrWhiteSpace(_scriptSeparator)) return new List<string> { sql };

			return Regex.Split(sql, $"^\\s*{_scriptSeparator}\\s*$", RegexOptions.Multiline)
						.Where(b => b.Trim().Length > 0)
						.Select(b => b.Trim())
						.ToList();
		}
	}
}