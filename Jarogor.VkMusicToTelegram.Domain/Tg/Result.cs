namespace Jarogor.VkMusicToTelegram.Domain.Tg;

public class Result {
    public string Group { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Link { get; set; } = null!;
    public int Views { get; set; }
    public int Reactions { get; set; }
}
