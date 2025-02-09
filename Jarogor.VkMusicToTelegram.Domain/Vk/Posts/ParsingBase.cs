using System.Text.RegularExpressions;
using Jarogor.VkMusicToTelegram.Domain.Vk.Api;

namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts;

public abstract class ParsingBase : IParsing {
    private static readonly Regex Regex = new(@"\[.+?|.+?\]", RegexOptions.Singleline, TimeSpan.FromMilliseconds(50));
    public abstract Result GetPreparedTitle(Post post);

    protected Result PrepareTitle(Post post) {
        var parts = post
            .Text
            ?.Split("\n")
            .Where(it => !string.IsNullOrWhiteSpace(it))
            .ToList();

        var name = parts?.FirstOrDefault();
        if (name is null) {
            return new NullResult();
        }

        var (success, artistName) = this.ParseArtistName(name);

        return success || !Regex.IsMatch(artistName)
            ? new Result(artistName, true)
            : new NullResult();
    }
}
