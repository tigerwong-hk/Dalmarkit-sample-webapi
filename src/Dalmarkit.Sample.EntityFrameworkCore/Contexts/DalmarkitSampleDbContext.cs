using Audit.EntityFramework;
using Dalmarkit.Blockchain.Constants;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.EntityFrameworkCore.Extensions;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dalmarkit.Sample.EntityFrameworkCore.Contexts;

public class DalmarkitSampleDbContext(DbContextOptions options) : AuditDbContext(options)
{
    private static readonly EnumToStringConverter<BlockchainNetwork> BlockchainNetworkConverter = new();

    public DbSet<ApiLog> ApiLogs { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<Entity> Entities { get; set; } = null!;
    public DbSet<EntityImage> EntityImages { get; set; } = null!;
    public DbSet<DependentEntity> DependentEntities { get; set; } = null!;
    public DbSet<EvmEvent> EvmEvents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.BuildApiLogEntity();

        _ = modelBuilder.BuildApiLogEntity();

        _ = modelBuilder.BuildReadWriteEntity<Entity>();
        _ = modelBuilder.Entity<Entity>()
            .Property(e => e.EntityId)
            .HasDefaultValueSql(ModelBuilderExtensions.DefaultGuidValueSql);
        _ = modelBuilder.Entity<Entity>()
            .HasIndex(e => e.EntityName)
            .HasFilter(@"""IsDeleted"" = false")
            .IsUnique();

        _ = modelBuilder.BuildMultipleReadWriteEntity<DependentEntity>();
        _ = modelBuilder.Entity<DependentEntity>()
            .Property(e => e.DependentEntityId)
            .HasDefaultValueSql(ModelBuilderExtensions.DefaultGuidValueSql);
        _ = modelBuilder.Entity<DependentEntity>()
            .HasIndex(e => new { e.DependentEntityName, e.EntityId })
            .HasFilter(@"""IsDeleted"" = false")
            .IsUnique();

        _ = modelBuilder.BuildReadWriteEntity<EntityImage>();
        _ = modelBuilder.Entity<EntityImage>()
            .Property(e => e.EntityImageId)
            .HasDefaultValueSql(ModelBuilderExtensions.DefaultGuidValueSql);
        _ = modelBuilder.Entity<EntityImage>()
            .HasIndex(e => new { e.ObjectName, e.EntityId })
            .HasFilter(@"""IsDeleted"" = false")
            .IsUnique();

        _ = modelBuilder.BuildReadOnlyEntity<EvmEvent>();
        _ = modelBuilder.Entity<EvmEvent>()
            .Property(e => e.EvmEventId)
            .HasDefaultValueSql(ModelBuilderExtensions.DefaultGuidValueSql);
        _ = modelBuilder.Entity<EvmEvent>()
            .Property(e => e.BlockchainNetwork)
            .HasConversion(BlockchainNetworkConverter)
            .HasMaxLength(20);

        base.OnModelCreating(modelBuilder);
    }
}
