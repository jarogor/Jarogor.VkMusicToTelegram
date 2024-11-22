namespace Jarogor.VkMusicToTelegram.PostHandlers;

public class Record(string name, bool isExists = false) {
    public string Name { get; } = name;
    public bool IsExists { get; } = isExists && !string.IsNullOrWhiteSpace(name);
}
