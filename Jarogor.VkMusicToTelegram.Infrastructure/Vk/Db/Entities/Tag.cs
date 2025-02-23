namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

public sealed class Tag {
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<AlbumTag> AlbumTags { get; set; } = [];
}