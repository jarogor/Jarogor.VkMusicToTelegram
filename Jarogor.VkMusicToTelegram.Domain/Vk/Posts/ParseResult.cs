namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts;

public readonly struct ParseResult(bool isSuccess, string[] tags, string[] artists, string album, int year, int? year2) {
    public bool IsSuccess { get; } = isSuccess;
    public string[] Tags { get; } = tags;
    public string[] Artists { get; } = artists;
    public string Album { get; } = album;
    public int Year { get; } = year;
    public int? Year2 { get; } = year2;

    public static ParseResult Create(string[] tags, string[] artists, string album, int year, int? year2)
        => new(true, tags, artists, album, year, year2);

    public static ParseResult Null()
        => new(false, [], [], string.Empty, 0, null);
}
