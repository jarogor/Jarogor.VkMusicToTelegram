namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

public sealed class Album {
    public int Id { get; set; }
    public required string Title { get; set; }
    public required ushort Year { get; set; }
    public ICollection<Post> Posts { get; set; } = [];

    public ICollection<AlbumArtist> AlbumArtists { get; set; } = [];
    public ICollection<AlbumTag> AlbumTags { get; set; } = [];
}