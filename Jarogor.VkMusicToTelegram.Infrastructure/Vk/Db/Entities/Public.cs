namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

public sealed class Public {
    public int Id { get; set; }
    public required string Domain { get; set; }
    public required string Title { get; set; }
    public required ICollection<Post>? Posts { get; set; } = [];
}
