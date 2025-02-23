using System.Collections.ObjectModel;

namespace Jarogor.VkMusicToTelegram.Domain.Vk.Api;

public interface IAdapter {
    public Task AuthorizeAsync(CancellationToken cancellationToken = new());
    public Task<Group> GetGroupAsync(string domain, CancellationToken cancellationToken = new());
    public Task<ReadOnlyCollection<Post>> GetPostsAsync(string domain, int count, CancellationToken cancellationToken = new());
    public Task<ReadOnlyCollection<Post>> GetPostsAsync(string domain, int count, int offset, CancellationToken cancellationToken = new());
}
