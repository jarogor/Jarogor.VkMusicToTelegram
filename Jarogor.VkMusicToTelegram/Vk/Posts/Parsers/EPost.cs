﻿using Jarogor.VkMusicToTelegram.Vk.Api;

namespace Jarogor.VkMusicToTelegram.Vk.Posts.Parsers;

/// <summary>
///     - первая строка это название `группа - альбом (год)`
///     - название группы может выглядеть так `[club787866|MINUALA]`, поэтому его нужно проверять регуляркой и вырезать
///     - последняя строка это теги
/// </summary>
public sealed class EPost : ParsingBase {
    public override string Domain => "post_music";
    public override string Name => @"E:\music\post";
    public override Record GetPreparedTitle(Post post) => PrepareTitle(post);
}
