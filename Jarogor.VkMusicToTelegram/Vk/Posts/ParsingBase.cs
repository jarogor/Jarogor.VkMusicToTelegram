﻿using System.Text.RegularExpressions;
using Jarogor.VkMusicToTelegram.Vk.Api;

namespace Jarogor.VkMusicToTelegram.Vk.Posts;

public abstract class ParsingBase : IParsing {
    private static readonly Regex Regex = new(@"\[.+?|.+?\]", RegexOptions.Singleline, TimeSpan.FromMilliseconds(50));

    public abstract string Domain { get; }
    public abstract string Name { get; }
    public abstract Record GetPreparedTitle(Post post);

    protected Record PrepareTitle(Post post) {
        var parts = post.Text
            .Split("\n")
            .Where(it => !string.IsNullOrWhiteSpace(it))
            .ToList();

        var name = parts.FirstOrDefault();
        if (name is null) {
            return new NullRecord();
        }

        var (success, artistName) = this.ParseArtistName(name);

        return success || !Regex.IsMatch(artistName)
            ? new Record(artistName, true)
            : new NullRecord();
    }
}
