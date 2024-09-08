namespace VkMusicToTelegram;

public sealed class Options
{
    public string VkApiAccessToken { get; set; } = null!;
    public string TgBotId { get; set; } = null!;
    public string TgChannelId { get; set; } = null!;
    public int VkCountPosts { get; set; } = 10;
    public int JobIntervalHours { get; set; } = 4;
    public int TopCount { get; set; } = 20;
}
