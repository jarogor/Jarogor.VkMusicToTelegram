using Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db;

public sealed class VkDbContext(DbContextOptions<VkDbContext> options) : DbContext(options) {
    public DbSet<Public> Publics { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<AlbumArtist> AlbumArtists { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<AlbumTag> AlbumTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Public>(entity => {
            entity.ToTable("public");
            entity.HasKey(it => it.Id);
            entity.Property(it => it.Id).HasColumnName("id").HasColumnType("BIGINT");
            entity.Property(it => it.Domain).HasColumnName("domain").HasMaxLength(100);
            entity.Property(it => it.Title).HasColumnName("title").HasMaxLength(200);
        });

        modelBuilder.Entity<Post>(entity => {
            entity.ToTable("post");
            entity.HasKey(it => it.Id);
            entity.Property(it => it.Id).HasColumnName("id").HasColumnType("BIGINT");
            entity.Property(it => it.AlbumId).HasColumnName("album_id");
            entity.Property(it => it.PublicId).HasColumnName("public_id");
            entity.Property(it => it.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz");
            entity.Property(it => it.Views).HasColumnName("views");
            entity.Property(it => it.Likes).HasColumnName("likes");
            entity.Property(it => it.Reposts).HasColumnName("reposts");

            entity
                .HasOne(it => it.Album)
                .WithMany(it => it.Posts)
                .HasForeignKey(it => it.AlbumId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_post_album");

            entity
                .HasOne(it => it.Public)
                .WithMany(it => it.Posts)
                .HasForeignKey(it => it.PublicId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_post_public");
        });

        modelBuilder.Entity<Artist>(entity => {
            entity.ToTable("artist");
            entity.HasKey(it => it.Id);
            entity.Property(it => it.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(it => it.Name).HasColumnName("name").HasMaxLength(200);
        });

        modelBuilder.Entity<Tag>(entity => {
            entity.ToTable("tag");
            entity.HasKey(it => it.Id);
            entity.Property(it => it.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(it => it.Name).HasColumnName("name").HasMaxLength(200);
        });

        modelBuilder.Entity<Album>(entity => {
            entity.ToTable("album");
            entity.HasKey(it => it.Id);
            entity.Property(it => it.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(it => it.Title).HasColumnName("title").IsRequired().HasMaxLength(200);
            entity.Property(it => it.Year).HasColumnName("year").HasColumnType("SMALLINT");
        });

        modelBuilder.Entity<AlbumArtist>(entity => {
            entity.ToTable("album_artist");
            entity.HasKey(it => new { it.AlbumId, it.ArtistId });
            entity.Property(it => it.AlbumId).HasColumnName("album_id").HasColumnType("int");
            entity.Property(it => it.ArtistId).HasColumnName("artist_id").HasColumnType("int");

            entity
                .HasOne(it => it.Album)
                .WithMany(it => it.AlbumArtists)
                .HasForeignKey(it => it.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(a => a.Artist)
                .WithMany(a => a.AlbumArtists)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AlbumTag>(entity => {
            entity.ToTable("album_tag");
            entity.HasKey(it => new { it.AlbumId, it.TagId });
            entity.Property(it => it.AlbumId).HasColumnName("album_id").HasColumnType("int");
            entity.Property(it => it.TagId).HasColumnName("tag_id").HasColumnType("int");

            entity
                .HasOne(it => it.Album)
                .WithMany(it => it.AlbumTags)
                .HasForeignKey(it => it.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(a => a.Tag)
                .WithMany(a => a.AlbumTags)
                .HasForeignKey(a => a.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
