using InfotecsBackend.Models.Emtities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfotecsBackend.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}
    
    public DbSet<Device> Devices { get; set; }
    public DbSet<Session> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.LastSeenAt)
                .IsRequired();

            builder.HasIndex(x => x.LastSeenAt)
                .HasDatabaseName("IX_Device_LastSeenAt");
            
            builder.HasMany(x => x.Sessions)
                .WithOne(x => x.Device)
                .HasForeignKey(x => x.DeviceId);
        });

        modelBuilder.Entity<Session>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Version)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.StartTime).IsRequired();
            builder.Property(x => x.EndTime).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasIndex(x => x.DeviceId)
                .HasDatabaseName("IX_Session_DeviceId");
            
            builder.HasIndex(x => new { x.DeviceId, x.StartTime })
                .HasDatabaseName("IX_Session_DeviceId_StartTime");

            builder.HasIndex(x => x.IsDeleted)
                .HasFilter("\"IsDeleted\" = false");
            
            builder.HasIndex(x => x.EndTime)
                .HasDatabaseName("IX_Session_EndTime");
            
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasOne(x => x.Device)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}