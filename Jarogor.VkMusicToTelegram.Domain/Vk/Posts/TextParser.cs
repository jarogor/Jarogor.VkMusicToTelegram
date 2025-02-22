using System.Text.RegularExpressions;

namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts;

/// <warning>PARTIAL</warning>
/// <summary>
/// Универсальный парсинг текста поста ВК, в котором ищутся: теги, артист, альбом, год.
/// </summary>
public static partial class TextParser {
    [GeneratedRegex(
        pattern: @"(?<artist>.*?)\s*[—–\-~]+\s*(?<album>.*)\s*\((?<year>\d{4})\/?(?<year2>\d{2,4})?[,|\/]?.*\)",
        options: RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        matchTimeoutMilliseconds: 50
    )]
    private static partial Regex MainRegex();

    [GeneratedRegex(
        pattern: @"(?:(\[[^|]+\|(?<artist>[^\]]+))(?:\s*?(feat\.?|vs\.?|\/|\|)?\s*?)?\1*)",
        options: RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        matchTimeoutMilliseconds: 50
    )]
    private static partial Regex ArtistRegex();

    [GeneratedRegex(
        pattern: @"\s*(?:feat\.?|vs\.?|[/\\|])\s*",
        options: RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        matchTimeoutMilliseconds: 50
    )]
    private static partial Regex ArtistSplitRegex();

    [GeneratedRegex(
        pattern: @"(?<=#)(?![0-9])\w+",
        options: RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        matchTimeoutMilliseconds: 50
    )]
    private static partial Regex TagsRegex();

    public static ParseResult Parse(string text) {
        var tags = TagsRegex()
            .Matches(text)
            .SelectMany(m => m.Groups.Values)
            .Select(m => m.Value)
            .ToArray();

        var match = MainRegex().Match(text);
        if (!match.Success) {
            return ParseResult.Null();
        }

        var artist = GetArtists(match.Groups["artist"].Value.Trim()).ToArray();
        var album = match.Groups["album"].Value.Trim();
        var year = int.Parse(match.Groups["year"].Value);
        var year2 = GetYear2(match.Groups["year2"].Value, year);

        return ParseResult.Create(tags, artist, album, year, year2);
    }

    private static string[] GetArtists(string input) {
        var match = ArtistRegex().Match(input);

        if (!match.Success) {
            return ArtistSplitRegex()
                .Split(input)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }

        var artists = new List<string>();

        while (match.Success) {
            var collection = match
                .Groups
                .Cast<System.Text.RegularExpressions.Group>()
                .Where(it => it.Name == "artist")
                .Where(it => !string.IsNullOrWhiteSpace(it.Value))
                .Select(it => it.Value.Trim())
                .ToArray();

            artists.AddRange(collection);
            match = match.NextMatch();
        }

        return artists.ToArray();
    }

    private static int? GetYear2(string data, int? yearBase) {
        int? result = int.TryParse(data, out var y) ? y : null;

        return result < 100
            ? yearBase - (yearBase % 100) + result
            : result;
    }
}
