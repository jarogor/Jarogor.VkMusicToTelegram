namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

public sealed class AlbumArtist {
    public int AlbumId { get; set; }
    public Album Album { get; set; }
    public int ArtistId { get; set; }
    public Artist Artist { get; set; }
}