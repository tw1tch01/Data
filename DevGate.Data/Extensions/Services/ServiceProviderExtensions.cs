using System;
using DevGate.Data.Contexts;
using DevGate.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevGate.Data.Extensions.Services
{
	/// <summary>
	/// Adds features to <see cref="IServiceProvider"/> for DevGate.Data
	/// </summary>
	public static class ServiceProviderExtensions
	{
		/// <summary>
		/// Calls the <see cref="DatabaseFacade.EnsureCreated"/> method on the DbContext
		/// </summary>
		/// <typeparam name="TContext">Context type</typeparam>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceProvider EnsureDatabaseCreated<TContext>(this IServiceProvider services) where TContext : DbContext, IDbContext
		{
			var log = services.GetService<ILogger<TContext>>() ?? new LoggerFactory().CreateLogger<TContext>();
			log.LogTrace($"Calling EnsureDatabaseCreated<{typeof(TContext)}>()");
			services.GetService<TContext>().Database.EnsureCreated();

			log.LogInformation($"EnsureCreated called on {typeof(TContext)}");

			return services;
		}

		/// <summary>
		/// Calls <see cref="DatabaseFacade"/>.Migrate() on the specified type.
		/// </summary>
		/// <typeparam name="TContext"></typeparam>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceProvider Migrate<TContext>(this IServiceProvider services) where TContext : DbContext, IDbContext
		{
			var log = services.GetService<ILogger<TContext>>() ?? new LoggerFactory().CreateLogger<TContext>();
			log.LogTrace($"Calling Migrate<{typeof(TContext)}>()");
			services.GetService<TContext>().Database.Migrate();

			log.LogInformation($"Migration called on {typeof(TContext)}");

			return services;
		}

		/// <summary>
		/// Calls all the seed methods on the registered <see cref="IDbContextSeeder"/> on the DbContext
		/// </summary>
		/// <typeparam name="TContext">Context type</typeparam>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceProvider Seed<TContext>(this IServiceProvider services) where TContext : DbContext, IDbContext
		{
			var log = services.GetService<ILogger<TContext>>() ?? new LoggerFactory().CreateLogger<TContext>();
			log.LogTrace($"Calling Seed<{typeof(TContext)}>()");
			var dbContext = services.GetService<TContext>();
			foreach (var seeder in services.GetServices<IDbContextSeeder<TContext>>())
			{
				seeder.Seed(dbContext);
			}

			return services;
		}
	}
}