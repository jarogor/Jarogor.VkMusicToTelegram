namespace Jarogor.VkMusicToTelegram.Domain.Vk.Api;

public sealed class Group {
    /// <summary>
    /// Идентификатор группы/паблика.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Название.
    /// </summary>
    public string Name { get; set; }
}
