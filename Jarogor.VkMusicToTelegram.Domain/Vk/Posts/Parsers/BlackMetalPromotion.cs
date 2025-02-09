using Jarogor.VkMusicToTelegram.Domain.Vk.Api;

namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts.Parsers;

/// <summary>
///     - первая строка это название `группа - альбом (описание)`
///     - последняя строка это теги
/// </summary>
public sealed class BlackMetalPromotion : IParsing {
    public Result GetPreparedTitle(Post post) {
        var parts = post
            .Text
            ?.Split("\n")
            .Where(it => !string.IsNullOrWhiteSpace(it))
            .ToList();

        var name = parts?.FirstOrDefault();

        return name is null
            ? new NullResult()
            : new Result(name, true);
    }
}
