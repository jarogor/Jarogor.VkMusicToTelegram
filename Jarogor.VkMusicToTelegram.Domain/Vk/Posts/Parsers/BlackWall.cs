using Jarogor.VkMusicToTelegram.Domain.Vk.Api;

namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts.Parsers;

/// <summary>
///     - первая строка это название `группа - альбом (год)`
///     - название группы может выглядеть так `[club787866|MINUALA]`
///     - последняя строка это теги, имеющие разделитель `|`
/// </summary>
public sealed class BlackWall : ParsingBase {
    public override Result GetPreparedTitle(Post post) => PrepareTitle(post);
}
