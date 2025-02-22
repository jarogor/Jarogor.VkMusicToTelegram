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
        var years = GetYears(match.Groups);

        return ParseResult.Create(tags, artist, album, years);
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

    private static int[] GetYears(GroupCollection groups) {
        var year = int.Parse(groups["year"].Value);

        if (!int.TryParse(groups["year2"].Value, out var year2)) {
            return [year];
        }

        // Если двузначное, то приведение к четырёхзначному на основе первого значения.
        // Например:
        //  - 1111/22 — в итоге должны стать 1111 и 1122
        //  - 1111/2222 — должны остаться как есть
        year2 = year2 < 100
            ? year - (year % 100) + year2
            : year2;

        return [year, year2];
    }
}
