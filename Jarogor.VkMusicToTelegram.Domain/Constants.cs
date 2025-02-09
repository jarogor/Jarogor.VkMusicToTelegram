using Jarogor.VkMusicToTelegram.Domain.Vk.Posts.Parsers;

namespace Jarogor.VkMusicToTelegram.Domain;

public static class Constants {
    public static readonly List<Group> VkGroupsSettings = [
        new("blackwall", "Blackwall", new BlackWall(), new TopCount(300, 50)),
        new("asylumforesters_vk", "Убежище Лесников", new AsylumForesters(), new TopCount(300, 50)),
        new("black_metal_promotion", "Black Metal Promotion", new BlackMetalPromotion(), new TopCount(25, 10)),
        new("e_black_metal", @"E:\music\black metal", new EBlackMetal(), new TopCount(25, 10)),
        new("post_music", @"E:\music\post", new EPost(), new TopCount(35, 15)),
        new("progressivemetal", @"E:\music\progressive metal", new EProgressiveMetal(), new TopCount(25, 10)),
    ];
}
