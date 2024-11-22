namespace Jarogor.VkMusicToTelegram.Jobs;

public class Item {
    public string Group { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Link { get; set; } = null!;
    public int Views { get; set; }
    public int Reactions { get; set; }
}
