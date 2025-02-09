using System.Collections.ObjectModel;

namespace Jarogor.VkMusicToTelegram.Domain.Vk.Api;

public interface IAdapter {
    public Task AuthorizeAsync(string accessToken, CancellationToken cancellationToken);
    public ReadOnlyCollection<Post> GetPosts(string domain, int count);
    public ReadOnlyCollection<Post> GetPosts(string domain, int count, int offset);
}
