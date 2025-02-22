using Jarogor.VkMusicToTelegram.Domain.Vk.Posts;

namespace Jarogor.VkMusicToTelegram.Tests.Infrastructure;

public class TextParserTest {
    const string Text =
        """
        some text here
        some text here
        [club1|a'b z] vs [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, m)
        some text here
        #some #text #here
        some text here
        """;

    [Fact]
    public void FullPostText_Parse_Success() {
        var result = TextParser.Parse(Text);
        Assert.True(result.IsSuccess);
        Assert.Equal(["some", "text", "here"], result.Tags);
        Assert.Equal(["a'b z", "c"], result.Artists);
        Assert.Equal("d. e & f'g: h, i-j - k?!", result.Album);
        Assert.Equal([1111, 1122], result.Years);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void MusicTextLine_Parse_Success(string value, object[] expected) {
        var result = TextParser.Parse(value);
        Assert.True(result.IsSuccess);
        Assert.Equal([], result.Tags);
        Assert.Equal(expected[0], result.Artists);
        Assert.Equal(expected[1], result.Album);
        Assert.Equal(expected[2], result.Years);
    }

    public static IEnumerable<object[]> TestData
        => new List<object[]> {
            new object[] { "a c — b (1111)", new object[] { new[] { "a c" }, "b", new[] { 1111 } } },
            new object[] { "foo — bar (1111/2222, l)", new object[] { new[] { "foo" }, "bar", new[] { 1111, 2222 } } },
            new object[] { "a'b z — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "a'b z / c / x — d. e & f'g: h, i-j - k?! (1111/22, m)", new object[] { new[] { "a'b z", "c", "x" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "a'b z feat. c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "a'b z feat c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "a'b z vs. c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "a'b z vs c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "a'b z / c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "a'b z | c — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "[club1|a'b z] — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "[club1|a'b z] / [club2|c] vs [club2|z] — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c", "z" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "[club1|a'b z] / [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "[club1|a'b z] | [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, l)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "[club1|a'b z] feat. [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, m)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "[club1|a'b z] feat [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, m)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "[club1|a'b z] vs. [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, m)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
            new object[] { "[club1|a'b z] vs [club2|c] — d. e & f'g: h, i-j - k?! (1111/22, m)", new object[] { new[] { "a'b z", "c" }, "d. e & f'g: h, i-j - k?!", new[] { 1111, 1122 } } },
        };
}
