using Jarogor.VkMusicToTelegram.Domain.Vk.Posts;

namespace Jarogor.VkMusicToTelegram.Domain;

public readonly struct Group(string domain, string name, IParsing parsing, TopCount top) {
    public string Domain { get; } = domain;
    public string Name { get; } = name;
    public IParsing Parsing { get; } = parsing;
    public TopCount Top { get; } = top;
}
