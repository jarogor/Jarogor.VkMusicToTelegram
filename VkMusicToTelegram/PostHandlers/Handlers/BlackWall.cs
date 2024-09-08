namespace VkMusicToTelegram.PostHandlers.Handlers;

/// <summary>
/// - первая строка это название `группа - альбом (год)`
/// - название группы может выглядеть так `[club787866|MINUALA]`, поэтому его нужно проверять регуляркой и вырезать
/// - последняя строка это теги, но имеющие разделитель `|` отделяющий теги музыки от каких-то служебных, можно пробовать отрезать
/// </summary>
public sealed class BlackWall : IHandler
{
    public string Domain => "blackwall";
    public string Name => "Blackwall";

    public Text GetPreparedTitle(Dto.Post post)
    {
        var parts = post.Text
            .Split("\n")
            .Where(it => !string.IsNullOrWhiteSpace(it))
            .ToList();

        var name = parts.FirstOrDefault();
        if (name is null)
        {
            return new Text();
        }

        var tags = parts.Count > 1
            ? parts.LastOrDefault() ?? string.Empty
            : string.Empty;

        (bool success, string artistName) = this.ParseArtistName(name);

        return success
            ? new Text(artistName, HandleTags(tags), true)
            : new Text();
    }

    private static string HandleTags(string? line)
    {
        return line?.Split('|').FirstOrDefault()?.Trim() ?? string.Empty;
    }
}