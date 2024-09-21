namespace VkMusicToTelegram.PostHandlers;

public class Record(string? name = null, bool isExists = false) {
    public string? Name { get; } = name;
    public bool IsExists { get; } = isExists;
};
