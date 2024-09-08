namespace VkMusicToTelegram.PostHandlers;

public interface IHandler
{
    public string Domain { get; }
    public string Name { get; }
    public Text GetPreparedTitle(Dto.Post post);
}

public record struct Text(string Name, string Tags, bool IsExists = false);