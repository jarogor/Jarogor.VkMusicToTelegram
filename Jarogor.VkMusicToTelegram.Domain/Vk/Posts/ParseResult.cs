namespace Jarogor.VkMusicToTelegram.Domain.Vk.Posts;

public readonly struct ParseResult(bool isSuccess, string[] tags, string[] artists, string album, int[] years) {
    public bool IsSuccess { get; } = isSuccess;
    public string[] Tags { get; } = tags;
    public string[] Artists { get; } = artists;
    public string Album { get; } = album;
    public int[] Years { get; } = years;

    public static ParseResult Create(string[] tags, string[] artists, string album, int[] years)
        => new(true, tags, artists, album, years);

    public static ParseResult Null()
        => new(false, [], [], string.Empty, null);
}
