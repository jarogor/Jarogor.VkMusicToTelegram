using Jarogor.VkMusicToTelegram.Domain.Vk.Api;

namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts;

public interface IParsing {
    public Result GetPreparedTitle(Post post);
}
