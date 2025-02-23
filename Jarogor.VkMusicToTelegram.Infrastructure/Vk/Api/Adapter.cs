using System.Collections.ObjectModel;
using Jarogor.VkMusicToTelegram.Domain.Vk.Api;
using Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models;
using Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models.JsonConverters;
using VkNet;
using VkNet.Model;
using VkNet.Utils;
using Post = Jarogor.VkMusicToTelegram.Domain.Vk.Api.Post;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api;

public sealed class Adapter(string accessToken, VkApi vkApi) : IAdapter {
    public async Task AuthorizeAsync(CancellationToken cancellationToken = new()) {
        await vkApi.AuthorizeAsync(new ApiAuthParams { AccessToken = accessToken }, cancellationToken);
    }

    public async Task<Public> GetPublicAsync(string domain, CancellationToken cancellationToken = new()) {
        var parameters = new VkParameters { { "group_id", domain } };
        var group = await vkApi.CallAsync<Group>("groups.getById", parameters, true, cancellationToken);
        return group.MapToPublic();
    }

    public async Task<ReadOnlyCollection<Post>> GetPostsAsync(string domain, int count, CancellationToken cancellationToken = new())
        => await WallGetAsync(new VkParameters {
            { "domain", domain },
            { "count", count },
        }, cancellationToken);

    public async Task<ReadOnlyCollection<Post>> GetPostsAsync(string domain, int count, int offset, CancellationToken cancellationToken = new())
        => await WallGetAsync(new VkParameters {
            { "domain", domain },
            { "count", count },
            { "offset", offset },
        }, cancellationToken);

    private Task<ReadOnlyCollection<Post>> WallGetAsync(VkParameters parameters, CancellationToken cancellationToken) {
        var posts = vkApi
            .Call<CustomWall>("wall.get", parameters, true, new CustomAttachmentJsonConverter())
            ?.WallPosts
            ?.Select(it => it.MapToPost())
            .ToReadOnlyCollection() ?? new ReadOnlyCollection<Post>([]);

        return Task
            .FromResult(posts)
            .WaitAsync(cancellationToken);
    }
}
