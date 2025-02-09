using Jarogor.VkMusicToTelegram.Domain.Vk.Api;
using Jarogor.VkMusicToTelegram.Domain.Vk.Posts;
using Microsoft.Extensions.Logging;

namespace Jarogor.VkMusicToTelegram.Domain.Tg.Top;

public abstract class TopServiceBase(ILogger<TopServiceBase> logger, TopOptions topOptions) {
    private readonly string _vkApiAccessToken = topOptions.VkApiAccessToken;
    private readonly int _tgTopCount = topOptions.TgTopCount;
    private readonly int _vkPostsLimit = topOptions.VkPostsLimit;
    private readonly Vk.Api.IAdapter _vkAdapter = topOptions.VkAdapter;
    private readonly IAdapter _tgAdapter = topOptions.TgAdapter;

    protected abstract TimeSpan Interval();
    protected abstract string Top { get; }
    protected abstract Task RunAsync(CancellationToken stoppingToken);

    public async Task ExecuteAsync(CancellationToken cancellationToken) {
        await _vkAdapter.AuthorizeAsync(_vkApiAccessToken, cancellationToken);
        await RunAsync(cancellationToken);
    }

    protected async Task HandleAsync(string domain, string groupName, IParsing parsing, int count, CancellationToken stoppingToken) {
        logger.LogDebug("{0}: domain: {1}, name: {2}, count: {3}", nameof(HandleAsync), domain, groupName, count);
        var topContent = new Dictionary<string, List<Result>>();

        foreach (var post in GetPosts(domain, count)) {
            if (post.IsSkip()) {
                continue;
            }

            var item = parsing.GetPreparedTitle(post);
            if (!item.IsExists) {
                continue;
            }

            if (!topContent.ContainsKey(groupName)) {
                logger.LogDebug("{0} _topContent groupName: {1}", nameof(HandleAsync), groupName);
                topContent[groupName] = [];
            }

            topContent[groupName].Add(post.CreateItem(groupName, item.Name));
        }

        if (topContent.Count <= 0) {
            return;
        }

        logger.LogDebug("{0}, topName {1}", nameof(HandleAsync), Top);
        var message = topContent.CreateTopMessage(_tgTopCount, Top);
        logger.LogDebug("{0}", message);
        await _tgAdapter.SendMessage(message, stoppingToken);
    }

    private List<Post> GetPosts(string domain, int count) {
        if (count <= _vkPostsLimit) {
            logger.LogDebug("{0}, domain: {1}, count: {2}, offset: {3}", nameof(GetPosts), domain, count, 0);
            return CreateIntervalPosts(
                _vkAdapter.GetPosts(domain, count, 0)
                    ?.OrderByDescending(it => it.Date)
                    .ToList() ?? []
            );
        }

        var posts = new List<Post>();

        while (true) {
            logger.LogDebug("{0}, domain: {1}, count: {2}, offset: {3}", nameof(GetPosts), domain, _vkPostsLimit, posts.Count);
            posts.AddRange(_vkAdapter.GetPosts(domain, _vkPostsLimit, posts.Count)?.ToList() ?? []);
            if (posts.Count >= count) {
                break;
            }
        }

        return CreateIntervalPosts(posts);
    }

    private List<Post> CreateIntervalPosts(List<Post> posts) {
        var orderByDescending = posts
            .OrderByDescending(it => it.Date)
            .ToList();

        var latestDate = orderByDescending[0].Date;
        var interval = Interval();

        logger.LogDebug("{0}, latestDate: {1}, interval: {2}", nameof(CreateIntervalPosts), latestDate, interval);

        return orderByDescending
            .TakeWhile(it => latestDate - it.Date < interval)
            .ToList();
    }
}
