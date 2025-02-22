using Jarogor.VkMusicToTelegram.Domain.Vk.Posts;

namespace Jarogor.VkMusicToTelegram.Tests.Infrastructure;

public class PostTextParserTest {
    [Theory]
    [MemberData(nameof(TestData))]
    public void Parse(string value, object[] expected) {
        var result = PostTextParser.Parse(value);

        Assert.Equal(expected[0], result.Artists);
        Assert.Equal(expected[1], result.Album);
        Assert.Equal(expected[2], result.Year);
        Assert.Equal(expected[3], result.Year2);
    }

    public static IEnumerable<object[]> TestData
        => new List<object[]> {
            new object[] { "a c — b (1111)", new object[] { new[] { "a c" }, "b", 1111, (int?)null } },
            new object[] { "foo — bar (1111/2222, l)", new object[] { new[] { "foo" }, "bar", 1111, 2222 }, },
            new object[] { "a'b z — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "a'b z / c / x — d. e & f'g: h, i-j - k?! (1111/22, m)", new object[] { new[] { "a'b z", "c", "x" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "a'b z feat. c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "a'b z feat c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "a'b z vs. c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "a'b z vs c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "a'b z / c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "a'b z | c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "[club1|a'b z] — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "[club1|a'b z] / [club2|c] vs [club2|z] — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c", "z" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "[club1|a'b z] / [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "[club1|a'b z] | [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "[club1|a'b z] feat. [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, m)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "[club1|a'b z] feat [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, m)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "[club1|a'b z] vs. [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, m)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
            new object[] { "[club1|a'b z] vs [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, m)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", 1111, 1122 } },
        };
}
