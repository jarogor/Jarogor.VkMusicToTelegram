using Jarogor.VkMusicToTelegram.Domain.Vk.Api;

namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts.Parsers;

/// <summary>
///     - первая строка это теги
///     - вторая непустая строка
///     - название из плейлиста, `группа - альбом (год)`
/// </summary>
public sealed class AsylumForesters : IParsing {
    public Result GetPreparedTitle(Post post) {
        return post.Attachments.Count <= 0
            ? new NullResult()
            : new Result(string.Join(",", post.Attachments), true);
    }
}
