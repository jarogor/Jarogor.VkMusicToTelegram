using System.Collections.ObjectModel;
using Jarogor.VkMusicToTelegram.Domain.Vk.Api;
using Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models;
using Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models.JsonConverters;
using VkNet;
using VkNet.Model;
using VkNet.Utils;
using Link = Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models.Link;
using Post = Jarogor.VkMusicToTelegram.Domain.Vk.Api.Post;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api;

public sealed class Adapter(string accessToken) : IAdapter {
    private readonly VkApi _vkApiClient = new();

    public async Task AuthorizeAsync(CancellationToken cancellationToken) {
        await _vkApiClient.AuthorizeAsync(new ApiAuthParams { AccessToken = accessToken }, cancellationToken);
    }

    public ReadOnlyCollection<Post> GetPosts(string domain, int count)
        => WallGet(new VkParameters {
            { "domain", domain },
            { "count", count },
        });

    public ReadOnlyCollection<Post> GetPosts(string domain, int count, int offset)
        => WallGet(new VkParameters {
            { "domain", domain },
            { "count", count },
            { "offset", offset },
        });

    private ReadOnlyCollection<Post> WallGet(VkParameters parameters) {
        return _vkApiClient
            .Call<CustomWall>("wall.get", parameters, true, new CustomAttachmentJsonConverter())
            ?.WallPosts
            ?.Select(Map)
            .ToReadOnlyCollection() ?? new ReadOnlyCollection<Post>([]);
    }

    private static Post Map(Models.Post it) {
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
