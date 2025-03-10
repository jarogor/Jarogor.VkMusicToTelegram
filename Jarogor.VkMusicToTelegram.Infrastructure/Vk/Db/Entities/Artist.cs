namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

public sealed class Artist {
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<AlbumArtist> AlbumArtists { get; set; } = [];
}