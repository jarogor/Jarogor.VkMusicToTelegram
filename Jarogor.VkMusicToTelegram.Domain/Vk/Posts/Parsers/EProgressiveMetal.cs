using Jarogor.VkMusicToTelegram.Domain.Vk.Api;

namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts.Parsers;

/// <summary>
///     - первая строка это название `группа - альбом (год)`
///     - название группы может выглядеть так `[club787866|MINUALA]`
///     - последняя строка это теги
/// </summary>
public sealed class EProgressiveMetal : ParsingBase {
    public override Result GetPreparedTitle(Post post) => PrepareTitle(post);
}
