namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

public sealed class AlbumTag {
    public int AlbumId { get; set; }
    public Album Album { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}