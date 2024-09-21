namespace VkMusicToTelegram;

public sealed class Options {
    public string VkApiAccessToken { get; set; } = null!;
    public string TgBotId { get; set; } = null!;
    public string TgChannelId { get; set; } = null!;

    public int VkLastCount { get; set; } = 20;

    public int TgTopCount { get; set; } = 3;
}
