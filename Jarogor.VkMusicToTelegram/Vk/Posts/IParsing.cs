using Jarogor.VkMusicToTelegram.Vk.Api;

namespace Jarogor.VkMusicToTelegram.Vk.Posts;

public interface IParsing {
    public string Domain { get; }
    public string Name { get; }
    public Record GetPreparedTitle(Post post);
}
