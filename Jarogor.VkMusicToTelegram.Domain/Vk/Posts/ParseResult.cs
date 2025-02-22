namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts;

public readonly struct ParseResult(bool isFilled, string[] artists, string album, int year, int? year2) {
    public bool IsFilled { get; } = isFilled;
    public string[] Artists { get; } = artists;
    public string Album { get; } = album;
    public int Year { get; } = year;
    public int? Year2 { get; } = year2;

    public static ParseResult Create(string[] artists, string album, int year, int? year2)
        => new(true, artists, album, year, year2);

    public static ParseResult Null()
        => new(false, [], string.Empty, 0, null);
}
