using Jarogor.VkMusicToTelegram.JsonConverters;
using Jarogor.VkMusicToTelegram.PostHandlers;
using Jarogor.VkMusicToTelegram.PostHandlers.Handlers;

namespace Jarogor.VkMusicToTelegram;

public static class Constants {
    public const int VkRequestLimit = 100;
    public static readonly CustomAttachmentJsonConverter CustomAttachmentJsonConverter = new();

    public static readonly List<Group> VkGroups = [
        new("blackwall", "Blackwall", new BlackWall(), new TopCount(300, 50)),
        new("asylumforesters_vk", "Убежище Лесников", new AsylumForesters(), new TopCount(300, 50)),
        new("black_metal_promotion", "Black Metal Promotion", new BlackMetalPromotion(), new TopCount(25, 10)),
        new("e_black_metal", @"E:\music\black metal", new EBlackMetal(), new TopCount(25, 10)),
        new("post_music", @"E:\music\post", new EPost(), new TopCount(35, 15)),
        new("progressivemetal", @"E:\music\progressive metal", new EProgressiveMetal(), new TopCount(25, 10)),
        new("ru_black_metal", "Русский блэк-метал", new RuBlackMetal(), new TopCount(100, 30)),
    ];
}

public record Group(string Domain, string Name, IHandler Handler, TopCount Top);
