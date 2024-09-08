using VkMusicToTelegram.Dto;
using VkMusicToTelegram.PostHandlers;
using VkMusicToTelegram.PostHandlers.Handlers;

namespace VkMusicToTelegram;

public static class Constants {
    public static readonly CustomAttachmentJsonConverter CustomAttachmentJsonConverter = new ();

    public static readonly List<(string domain, string name, IHandler handler)> VkGroups = [
        ("blackwall", "Blackwall", new BlackWall()),
        ("asylumforesters_vk", "Убежище Лесников", new AsylumForesters()),
        ("black_metal_promotion", "Black Metal Promotion", new BlackMetalPromotion()),
        ("e_black_metal", @"E:\music\black metal", new EBlackMetal()),
        ("ru_black_metal", "Русский блэк-метал", new RuBlackMetal())
    ];
}
