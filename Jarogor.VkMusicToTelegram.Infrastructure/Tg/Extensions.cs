namespace Jarogor.VkMusicToTelegram.Infrastructure.Tg;

public static class Extensions {
    public static (Dictionary<string, List<T>> first, Dictionary<string, List<T>> second) Split<T>(this Dictionary<string, List<T>> items, int limit) {
        var first = new Dictionary<string, List<T>>();
        var second = new Dictionary<string, List<T>>();

        foreach (var (key, list) in items) {
            first[key] = list.Take(limit).ToList();
            second[key] = list.Skip(limit).ToList();
        }

        return (first, second);
    }
}
