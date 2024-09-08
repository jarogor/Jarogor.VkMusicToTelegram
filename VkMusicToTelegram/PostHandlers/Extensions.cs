using System.Text.RegularExpressions;

namespace VkMusicToTelegram.PostHandlers;

public static class Extensions
{
    private static readonly Regex Regex = new(@"^\[.+\|(.+)\]([\w\W]+)$", RegexOptions.Singleline, TimeSpan.FromMilliseconds(50));

    public static string ParseArtistName(this IHandler _, string data)
    {
        try
        {
            var match = Regex.Match(data);
            return match.Length == 0
                ? data
                : (match.Groups[1].Value + match.Groups[2].Value).Trim();
        }
        catch
        {
            return data;
        }
    }
}