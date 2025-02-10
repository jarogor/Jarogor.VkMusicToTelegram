namespace Jarogor.VkMusicToTelegram.Domain.Tg.Last;

public sealed class LastOptions {
    public int VkPostsCount { get; init; }
    public Vk.Api.IAdapter VkAdapter { get; init; } = null!;
    public IAdapter TgAdapter { get; init; } = null!;
    public List<Group> Groups { get; init; } = null!;
}
