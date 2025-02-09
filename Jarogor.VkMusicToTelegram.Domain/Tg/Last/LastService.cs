using Jarogor.VkMusicToTelegram.Domain.Vk.Api;
using Microsoft.Extensions.Logging;
using File = System.IO.File;

namespace Jarogor.VkMusicToTelegram.Domain.Tg.Last;

public sealed class LastService(ILogger<LastService> logger, LastOptions topOptions) {
    private readonly string _vkApiAccessToken = topOptions.VkApiAccessToken;
    private readonly int _vkLastCount = topOptions.VkPostsCount;
    private readonly Vk.Api.IAdapter _vkAdapter = topOptions.VkAdapter;
    private readonly IAdapter _tgAdapter = topOptions.TgAdapter;
    private readonly List<Group> _vkGroups = topOptions.Groups;

    private static string HistoryListFilePath => Path.Join(AppDomain.CurrentDomain.BaseDirectory, $"history-list-{DateTime.Now.Month}.txt");

    public async Task RunAsync(CancellationToken stoppingToken) {
        await _vkAdapter.AuthorizeAsync(_vkApiAccessToken, stoppingToken);

        var newContent = _vkGroups.ToDictionary(group => group.Name, _ => new List<Result>());
        var history = await GetHistory(stoppingToken);

        var newHistory = ReceiveAndHandlePosts(history, newContent);
        if (newHistory.Count == 0) {
            return;
        }

        // Отправка в Телеграм
        await _tgAdapter.SendMessage(newContent, stoppingToken);
        logger.LogDebug("new items count: {0}", newHistory.Count);

        // Добавление новых записей в историю
        history.AddRange(newHistory);
        await File.WriteAllLinesAsync(HistoryListFilePath, history, stoppingToken);
    }

    private List<string> ReceiveAndHandlePosts(List<string> history, Dictionary<string, List<Result>> newContent) {
        var newHistory = new List<string>();

        foreach (var group in _vkGroups) {
            var posts = _vkAdapter.GetPosts(group.Domain, _vkLastCount);
            logger.LogDebug("{0}: domain: {1}, name: {2}, count: {3}", nameof(RunAsync), group.Domain, group.Name, posts.Count);

            foreach (var post in posts) {
                if (post.IsSkip()) {
                    continue;
                }

                // После обработки названия
                var text = group.Parsing.GetPreparedTitle(post);
                if (!text.IsExists) {
                    continue;
                }

                // Если уже публиковался ранее
                if (history.Contains(text.Name)) {
                    continue;
                }

                newHistory.Add(text.Name);
                newContent[group.Name].Add(post.CreateItem(group.Name, text.Name));
            }
        }

        return newHistory;
    }

    private static async Task<List<string>> GetHistory(CancellationToken stoppingToken)
        => File.Exists(HistoryListFilePath)
            ? (await File.ReadAllLinesAsync(HistoryListFilePath, stoppingToken)).ToList()
            : [];
}
