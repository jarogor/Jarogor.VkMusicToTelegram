namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts;

public class Result(string name, bool isExists = false) {
    public string Name { get; } = name;
    public bool IsExists { get; } = isExists && !string.IsNullOrWhiteSpace(name);
}
