using Jarogor.VkMusicToTelegram.Dto;

namespace Jarogor.VkMusicToTelegram.PostHandlers;

public interface IHandler {
    public string Domain { get; }
    public string Name { get; }
    public Record GetPreparedTitle(Post post);
}
