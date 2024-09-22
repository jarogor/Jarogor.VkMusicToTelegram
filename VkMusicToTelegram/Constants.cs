using VkMusicToTelegram.Dto;
using VkMusicToTelegram.PostHandlers;
using VkMusicToTelegram.PostHandlers.Handlers;

namespace VkMusicToTelegram;

public static class Constants {
    public static readonly int VkRequestLimit = 100;
    public static readonly CustomAttachmentJsonConverter CustomAttachmentJsonConverter = new();

    public static readonly List<(string domain, string name, IHandler handler, TopCount top)> VkGroups = [
        ("blackwall", "Blackwall", new BlackWall(), new TopCount(300, 50)),
        ("asylumforesters_vk", "Убежище Лесников", new AsylumForesters(), new TopCount(300, 50)),
        ("black_metal_promotion", "Black Metal Promotion", new BlackMetalPromotion(), new TopCount(25, 10)),
        ("e_black_metal", @"E:\music\black metal", new EBlackMetal(), new TopCount(25, 10)),
        ("post_music", @"E:\music\post", new EPost(), new TopCount(35, 15)),
        ("progressivemetal", @"E:\music\progressive metal", new EProgressiveMetal(), new TopCount(25, 10)),
        ("ru_black_metal", "Русский блэк-метал", new RuBlackMetal(), new TopCount(100, 30)),
    ];
}
