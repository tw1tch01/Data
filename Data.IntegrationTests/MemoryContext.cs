using System;
using System.Collections.Generic;
using AutoFixture;
using Data.Common;
using Data.Contexts;
using Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data.IntegrationTests
{
    internal class MemoryContext : DbContext, IAuditedContext
    {
        private const string CreatedBy = "IntegrationTests";
        private const string CreatedProcess = "/IntegrationTests";
        private const string ModifiedBy = "IntegrationTests";
        private const string ModifiedProcess = "/IntegrationTests";
        private readonly Fixture _fixture = new Fixture();

        public MemoryContext()
            : base(new DbContextOptionsBuilder<MemoryContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options)
        {
            ContextScope = new ContextScope();
            ContextScope.StateActions.Add(EntityState.Added, SetCreatedAuditFields);
            ContextScope.StateActions.Add(EntityState.Modified, SetModifiedAuditFields);
        }

        public ContextScope ContextScope { get; }

        public DbSet<AuditedEntity> AuditedClasses { get; }

        public void Seed()
        {
            var entities = new List<AuditedEntity>();
            for (int i = 0; i < 100; i++)
            {
                entities.Add(new AuditedEntity { Id = Guid.NewGuid(), CreatedBy = _fixture.Create<string>(), CreatedOn = _fixture.Create<DateTime>(), CreatedProcess = _fixture.Create<string>(), Name = _fixture.Create<string>() });
            }

            AddRange(entities);
            SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        private void SetCreatedAuditFields(EntityEntry entity)
        {
            entity.Entity.TrySetProperty(nameof(AuditedEntity.CreatedBy), CreatedBy)
                         .TrySetProperty(nameof(AuditedEntity.CreatedOn), DateTime.UtcNow)
                         .TrySetProperty(nameof(AuditedEntity.CreatedProcess), CreatedProcess);
        }

        private void SetModifiedAuditFields(EntityEntry entity)
        {
            entity.Entity.TrySetProperty(nameof(AuditedEntity.LastModifiedBy), ModifiedBy)
                         .TrySetProperty(nameof(AuditedEntity.LastModifiedOn), DateTime.UtcNow)
                         .TrySetProperty(nameof(AuditedEntity.LastModifiedProcess), ModifiedProcess);
        }
    }
}