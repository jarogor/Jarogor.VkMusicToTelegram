﻿using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VkNet.Model;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models;

[Serializable]
public sealed class CustomWall {
    /// <summary>
    ///     Общее количество записей на стене.
    /// </summary>
    [JsonProperty("count")]
    public ulong TotalCount { get; set; }

    /// <summary>
    ///     Посты.
    /// </summary>
    [JsonProperty("items")]
    public ReadOnlyCollection<Post>? WallPosts { get; set; }

    /// <summary>
    ///     Профили.
    /// </summary>
    [JsonProperty("profiles")]
    public ReadOnlyCollection<User>? Profiles { get; set; }

    /// <summary>
    ///     Группы.
    /// </summary>
    [JsonProperty("groups")]
    public ReadOnlyCollection<Group>? Groups { get; set; }
}
