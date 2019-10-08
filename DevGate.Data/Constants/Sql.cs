namespace DevGate.Data.Constants
{
	/// <summary>
	/// Constants for SQL Seeder
	/// </summary>
	internal static class Sql
	{
		public static class Separator
		{
			public const string Go = "GO";
		}

		/// <summary>
		/// File suffixes
		/// </summary>
		public static class Suffixes
		{
			/// <summary>
			/// Functions
			/// </summary>
			public const string Functions = ".function.sql";

			/// <summary>
			/// Procudures
			/// </summary>
			public const string Procudures = ".procedure.sql";

			/// <summary>
			/// Synonyms
			/// </summary>
			public const string Synonyms = ".synonym.sql";

			/// <summary>
			/// Triggers
			/// </summary>
			public const string Triggers = ".ddltrigger.sql";

			/// <summary>
			/// Views
			/// </summary>
			public const string Views = ".view.sql";
		}
	}
}