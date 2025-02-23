using System.Collections.ObjectModel;
using VkNet.Utils;
using Group = Jarogor.VkMusicToTelegram.Domain.Vk.Api.Group;
using Link = Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models.Link;
using Post = Jarogor.VkMusicToTelegram.Domain.Vk.Api.Post;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api;

public static class MapExtensions {
    public static Group MapToGroup(this VkNet.Model.Group it) {
        return new Group {
            Id = it.Id,
            Name = it.Name,
        };
    }

    public static Post MapToPost(this Models.Post it) {
        return new Post {
            Id = it.Id!.Value,
            Date = it.Date!.Value,
            IsPinned = it.IsPinned,
            OwnerId = it.OwnerId!.Value,
            Views = it.Views.Count,
            Likes = it.Likes.Count,
            Reposts = it.Reposts.Count,
            Attachments = it.Attachments
                ?.Where(a => a.Type == typeof(Link))
                .Select(a => a.Instance)
                .Cast<Link>()
                .Where(a => a.IsAudioPlaylist)
                .Where(a => a.Title is not null)
                .Select(a => a.Title)
                .ToReadOnlyCollection() ?? ReadOnlyCollection<string>.Empty,
            Text = it.Text,
        };
    }
}
