namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

public sealed class Public {
    public long Id { get; set; }
    public required string Domain { get; set; }
    public required string Title { get; set; }
    public ICollection<Post>? Posts { get; set; } = [];
}
