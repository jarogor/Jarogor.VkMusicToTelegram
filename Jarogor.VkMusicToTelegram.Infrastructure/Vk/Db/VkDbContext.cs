using Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db;

public sealed class VkDbContext(DbContextOptions<VkDbContext> options) : DbContext(options) {
    public DbSet<Band> Bands { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Public> Publics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Public>()
            .HasMany(it => it.Posts)
            .WithOne(it => it.Public);

        modelBuilder.Entity<Band>()
            .HasMany(it => it.Albums)
            .WithOne(it => it.Band);

        modelBuilder.Entity<Post>()
            .HasOne(it => it.Album)
            .WithMany(it => it.Posts);
    }
}
