using System;
using System.Reflection;
using DevGate.Data.Contexts;
using DevGate.Data.Other;
using DevGate.Data.Repositories;
using DevGate.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevGate.Data.Extensions.Services
{
	/// <summary>
	/// Extends the IServiceCollection with methods that enhance DevGate.Data
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		private static bool addedAuditFields = false;

		/// <summary>
		/// Adds a default implementation of <see cref="AuditFields"/> to the service collection.
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddAuditField(this IServiceCollection services)
		{
			if (addedAuditFields) return services;

			services.AddSingleton(new AuditFields());
			addedAuditFields = true;

			return services;
		}

		/// <summary>
		/// Adds DbContext, EntityRepository, and sql seeder
		/// </summary>
		/// <typeparam name="TContext">The type to add data services to.</typeparam>
		/// <param name="services"></param>
		/// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
		/// <returns></returns>
		public static IServiceCollection AddDataServices<TContext>(
			this IServiceCollection services,
			Action<DbContextOptionsBuilder> optionsAction)
				where TContext : DbContext, IDbContext
		{
			return AddDataServices<TContext, EntityRepository<TContext>>(services, optionsAction);
		}

		/// <summary>
		/// Adds DbContext, custom IEntityRepository, and sql seeder
		/// </summary>
		/// <typeparam name="TContext">The type to add data services to.</typeparam>
		/// <typeparam name="TRepository">The repository to use for the DbContext that is registered in these data services.</typeparam>
		/// <param name="services"></param>
		/// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
		/// <returns></returns>
		public static IServiceCollection AddDataServices<TContext, TRepository>(
			this IServiceCollection services,
			Action<DbContextOptionsBuilder> optionsAction)
				where TContext : DbContext, IDbContext
				where TRepository : class, IEntityRepository<TContext>
		{
			services.AddTransient<IEntityRepository<TContext>, TRepository>();
			services.AddAuditField();
			services.AddDbContext<TContext>(optionsAction);

			services.AddSeeder<SqlSeeder<TContext>, TContext>(new SqlSeederOptions<TContext>
			{
				Assembly = Assembly.GetAssembly(typeof(TContext))
			});

			return services;
		}


		/// <summary>
		/// Adds a new database seeder to the service collection with the specified options.
		/// </summary>
		/// <typeparam name="TSeeder"></typeparam>
		/// <typeparam name="TContext"></typeparam>
		/// <param name="services"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static IServiceCollection AddSeeder<TSeeder, TContext>(
			this IServiceCollection services,
			IDbContextSeederOptions<TContext> options = null)
				where TSeeder : class, IDbContextSeeder, IDbContextSeeder<TContext>
				where TContext : DbContext, IDbContext
		{
			if (options != null)
			{
				services.AddSingleton<IDbContextSeeder<TContext>, TSeeder>();
				services.AddSingleton(options.GetType(), options);
			}

			services.AddSingleton(typeof(TSeeder));
			return services;
		}
	}
}
