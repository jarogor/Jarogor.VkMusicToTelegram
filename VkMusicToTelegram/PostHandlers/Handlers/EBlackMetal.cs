using VkMusicToTelegram.Dto;

namespace VkMusicToTelegram.PostHandlers.Handlers;

/// <summary>
/// - первая строка это название `группа - альбом (год)`
/// - название группы может выглядеть так `[club787866|MINUALA]`, поэтому его нужно проверять регуляркой и вырезать
/// - последняя строка это теги
/// </summary>
public sealed class EBlackMetal : IHandler
{
    public string Domain => "e_black_metal";
    public string Name => @"E:\music\black metal";

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

        return name is null
            ? new Text()
            : new Text(this.ParseArtistName(name), tags, true);
    }
}