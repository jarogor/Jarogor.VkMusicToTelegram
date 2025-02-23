namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

public sealed class Post {
    public long Id { get; set; }
    public int AlbumId { get; set; }
    public int GroupId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required int Views { get; set; }
    public required int Likes { get; set; }
    public required int Reposts { get; set; }
    public Group? Group { get; set; }
    public Album? Album { get; set; }
}
