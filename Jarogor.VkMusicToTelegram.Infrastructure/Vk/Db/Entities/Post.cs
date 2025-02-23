namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

public sealed class Post {
    public long Id { get; set; }
    public required int AlbumId { get; set; }
    public required int PublicId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required int Views { get; set; }
    public required int Likes { get; set; }
    public required int Reposts { get; set; }
    public Public? Public { get; set; }
    public Album? Album { get; set; }
}