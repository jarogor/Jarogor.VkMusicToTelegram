using Jarogor.VkMusicToTelegram.Dto;

namespace Jarogor.VkMusicToTelegram.PostHandlers.Handlers;

/// <summary>
/// - первая строка название вида `[club159431308|Ad Nihil] - Заведомо ложные цели (2024)`, нужно вырезать лишнее
/// - последняя строка это теги
/// </summary>
public sealed class RuBlackMetal : Handler1Base {
    public override string Domain => "ru_black_metal";
    public override string Name => "Русский блэк-метал";
    public override Record GetPreparedTitle(Post post) => base.PrepareTitle(post);
}
