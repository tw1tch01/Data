using System;

namespace DevGate.Data.Migrations
{
	/// <summary>
	/// Migration methods
	/// </summary>
	public interface IMigratable
	{
		/// <summary>
		/// Performs a migration on the object using the specified service provider
		/// </summary>
		/// <param name="services"></param>
		void Migrate(IServiceProvider services);

		/// <summary>
		/// Performs a database seed ob the object using the service provider
		/// </summary>
		/// <param name="services"></param>
		void SeedDatabase(IServiceProvider services);
	}
}