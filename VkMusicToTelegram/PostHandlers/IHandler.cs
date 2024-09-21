namespace VkMusicToTelegram.PostHandlers;

public interface IHandler {
    public string Domain { get; }
    public string Name { get; }
    public Record GetPreparedTitle(Dto.Post post);
}
