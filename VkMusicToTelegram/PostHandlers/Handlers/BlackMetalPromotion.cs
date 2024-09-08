﻿using VkMusicToTelegram.Dto;

namespace VkMusicToTelegram.PostHandlers.Handlers;

/// <summary>
/// - первая строка это название `группа - альбом (описание)`
/// - последняя строка это теги, но первый служебный, а остальные какие-то наркоманские, быть может отказаться от тегов тут
/// </summary>
public sealed class BlackMetalPromotion : IHandler
{
    public string Domain => "black_metal_promotion";
    public string Name => "Black Metal Promotion";

    public Text GetPreparedTitle(Post post)
    {
        var parts = post.Text
            .Split("\n")
            .Where(it => !string.IsNullOrWhiteSpace(it))
            .ToList();

        var name = parts.FirstOrDefault();

        return name is null
            ? new Text()
            : new Text(name, string.Empty, true);
    }
}