using System;
using System.Data.Common;
using DevGate.Data.Contexts;
using DevGate.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevGate.Data.Tests
{
	public class DatabaseFactory<TContext> : IDisposable where TContext : DbContext
	{
		#region Fields

		private ILoggerFactory _loggerFactory;

		#endregion Fields

		#region Properties

		public DbConnection Connection { get; private set; }

		public ILoggerFactory LoggerFactory
		{
			get
			{
				if (_loggerFactory != null) return _loggerFactory;

				_loggerFactory = new LoggerFactory();

				return _loggerFactory;
			}

			set => _loggerFactory = value;
		}

		public DbContextOptionsBuilder<TContext> OptionsBuilder { get; private set; }

		#endregion Properties

		public static DatabaseFactory<TContext> CreateSqlLiteDatabase(ILoggerFactory loggerFactory = null)
		{
			var connection = new SqliteConnection("DataSource=:memory:");
			var factory = new DatabaseFactory<TContext>
			{
				LoggerFactory = loggerFactory,
				Connection = connection,
				OptionsBuilder = new DbContextOptionsBuilder<TContext>().UseSqlite(connection)
			};
			factory.Connection.Open();

			using (var context = factory.CreateDbContext())
			{
				context.Database.EnsureCreated();
			}

			return factory;
		}

		public TContext CreateDbContext()
		{
			if (typeof(TestDbContext).IsAssignableFrom(typeof(TContext)))
			{
				return new TestDbContext(OptionsBuilder.Options as DbContextOptions<TestDbContext>, LoggerFactory) as TContext;
			}

			return Activator.CreateInstance(typeof(TContext), OptionsBuilder.Options) as TContext;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_loggerFactory.Dispose();
				Connection.Dispose();
			}
		}

		public class TestDbContext : DbContext, IDbContext
		{
			private readonly ILoggerFactory _loggerFactory;

			public TestDbContext
			(
				DbContextOptions<TestDbContext> options, 
				ILoggerFactory loggerFactory
			) : base(options)
			{
				_loggerFactory = loggerFactory;
			}

			public new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
			{
				return base.Set<TEntity>();
			}
		}
	}
}