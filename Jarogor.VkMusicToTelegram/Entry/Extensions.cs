using System.Text.RegularExpressions;

namespace Jarogor.VkMusicToTelegram.Entry;

public static class Extensions {
    private static readonly Regex Regex = new(@"^\[[^|]+?\|([^|]+?)\](\s{1,3}[-—–~]{1,3}\s{1,3}[\w\W]+?[\(\[\{]{1}\d+?\w*?[\)\]\}]{1})[\w\W]*?$", RegexOptions.Singleline, TimeSpan.FromMilliseconds(50));

    public static (bool success, string artistName) ParseArtistName(this IHandler _, string data) {
        try {
            if (!data.Contains('|')) {
                return (false, data);
            }

            var match = Regex.Match(data);
            return match.Length == 0
                ? (false, data)
                : (true, (match.Groups[1].Value + match.Groups[2].Value).Trim());
        } catch {
            return (false, data);
        }
    }
}
