namespace Jarogor.VkMusicToTelegram.Domain.Tg.Top;

public sealed class TopOptions {
    public int TgTopCount { get; init; }
    public int VkPostsLimit { get; init; }
    public Vk.Api.IAdapter VkAdapter { get; init; } = null!;
    public IAdapter TgAdapter { get; init; } = null!;
    public List<Group> Groups { get; init; } = null!;
}
