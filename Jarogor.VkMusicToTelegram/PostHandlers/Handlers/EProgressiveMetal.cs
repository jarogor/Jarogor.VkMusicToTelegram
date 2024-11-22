﻿using Jarogor.VkMusicToTelegram.Dto;

namespace Jarogor.VkMusicToTelegram.PostHandlers.Handlers;

/// <summary>
///     - первая строка это название `группа - альбом (год)`
///     - название группы может выглядеть так `[club787866|MINUALA]`, поэтому его нужно проверять регуляркой и вырезать
///     - последняя строка это теги
/// </summary>
public sealed class EProgressiveMetal : Handler1Base {
    public override string Domain => "progressivemetal";
    public override string Name => @"E:\music\progressive metal";
    public override Record GetPreparedTitle(Post post) => PrepareTitle(post);
}
