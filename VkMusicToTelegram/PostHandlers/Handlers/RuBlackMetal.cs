using VkMusicToTelegram.Dto;

namespace VkMusicToTelegram.PostHandlers.Handlers;

/// <summary>
/// - первая строка название вида `[club159431308|Ad Nihil] - Заведомо ложные цели (2024)`, нужно вырезать лишнее
/// - последняя строка это теги
/// </summary>
public sealed class RuBlackMetal : IHandler
{
    public string Domain => "ru_black_metal";
    public string Name => "Русский блэк-метал";

    public Text GetPreparedTitle(Post post)
    {
        var parts = post.Text
            .Split("\n")
            .Where(it => !string.IsNullOrWhiteSpace(it))
            .ToList();

        var tags = parts.Count > 1
            ? parts.LastOrDefault() ?? string.Empty
            : string.Empty;

        var name = parts.FirstOrDefault();
        if (name is null)
        {
            return new Text();
        }

        (bool success, string artistName) = this.ParseArtistName(name);
        return success
            ? new Text(artistName, tags, true)
            : new Text();
    }
}