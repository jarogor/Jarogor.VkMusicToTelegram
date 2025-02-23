using Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db;

public sealed class VkDbContext(DbContextOptions<VkDbContext> options) : DbContext(options) {
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Public> Publics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Public>()
            .HasMany(it => it.Posts)
            .WithOne(it => it.Public);

        modelBuilder.Entity<Post>()
            .HasOne(it => it.Album)
            .WithMany(it => it.Posts);

        modelBuilder.Entity<Album>()
            .HasMany(it => it.Artists)
            .WithMany(it => it.Albums);
    }
}
