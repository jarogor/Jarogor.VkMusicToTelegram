using Jarogor.VkMusicToTelegram.Infrastructure.Tg;

namespace Jarogor.VkMusicToTelegram.Tests.Infrastructure;

public class TgMessageCollectionSplitTest {
    [Fact]
    public void Split() {
        var items = new Dictionary<string, List<int>> {
            ["a"] = [1, 2, 3],
            ["b"] = [1, 2, 3, 4],
            ["c"] = [1, 2, 3, 4, 5, 6, 7, 8, 9],
        };

        var (first, second) = items.Split(3);
        foreach (var pair in first) {
            Assert.Equal(3, pair.Value.Count);
        }

        Assert.Empty(second["a"]);
        Assert.Single(second["b"]);
        Assert.Equal(6, second["c"].Count);
    }
}
