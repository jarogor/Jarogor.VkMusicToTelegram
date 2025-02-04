using System.Collections.ObjectModel;
using System.Text;
using Jarogor.VkMusicToTelegram.Dto;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VkNet;
using VkNet.Model;
using VkNet.Utils;
using Dto_Link = Jarogor.VkMusicToTelegram.Dto.Link;
using File = System.IO.File;
using Post = Jarogor.VkMusicToTelegram.Dto.Post;

namespace Jarogor.VkMusicToTelegram.Jobs;

[DisallowConcurrentExecution]
public sealed class LastJob(ILogger<LastJob> logger, IOptions<Options> options) : IJob {
    private static string HistoryListFilePath => Path.Join(AppDomain.CurrentDomain.BaseDirectory, $"history-list-{DateTime.Now.Month}.txt");
    private readonly TelegramBotClient _tgApiClient = new(options.Value.TgBotId);
    private readonly string _tgChannelId = options.Value.TgChannelId;
    private readonly ApiAuthParams _vkApiAuthParams = new() { AccessToken = options.Value.VkApiAccessToken };
    private readonly VkApi _vkApiClient = new();

    private readonly int _vkLastCount = options.Value.VkLastCount;

    public Task Execute(IJobExecutionContext context)
        => Run(context.CancellationToken);

    private async Task Run(CancellationToken stoppingToken) {
        await _vkApiClient.AuthorizeAsync(_vkApiAuthParams, stoppingToken);

        var newContent = Constants.VkGroups.ToDictionary(group => group.Name, _ => new List<Item>());
        var history = await GetHistory(stoppingToken);
        var newHistory = new List<string>();

        foreach (var group in Constants.VkGroups) {
            foreach (var post in GetPosts(group)) {
                HandlePost(post, group, history, newHistory, newContent);
            }
        }

        if (newHistory.Count <= 0) {
            return;
        }

        // Отправка в Телеграм
        await _tgApiClient.SendMessage(
            _tgChannelId,
            CreateMessage(newContent, newHistory).ToString(),
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
            parseMode: ParseMode.Html,
            cancellationToken: stoppingToken
        );

        // Добавление новых записей в историю
        history.AddRange(newHistory);
        await File.WriteAllLinesAsync(HistoryListFilePath, history, stoppingToken);
    }

    // Формирование сообщения
    private StringBuilder CreateMessage(Dictionary<string, List<Item>> newContent, List<string> newHistory) {
        var message = new StringBuilder();
        var items = newContent.Where(pair => pair.Value.Count > 0);

        foreach (var pair in items) {
            message.AppendLine(pair.Key);
            foreach (var item in pair.Value) {
                message.AppendLine($"""- <a href="{item.Link}">{item.Name}</a>""");
            }

            message.AppendLine();
        }

        logger.LogInformation("new items count: {0}", newHistory.Count);

        return message;
    }

    private static async Task<List<string>> GetHistory(CancellationToken stoppingToken)
        => File.Exists(HistoryListFilePath)
            ? (await File.ReadAllLinesAsync(HistoryListFilePath, stoppingToken)).ToList()
            : [];

    private ReadOnlyCollection<Post> GetPosts(Group group) {
        var vkParameters = new VkParameters {
            { "domain", group.Domain },
            { "count", _vkLastCount },
        };

        var posts = _vkApiClient
            .Call<CustomWall>("wall.get", vkParameters, true, Constants.CustomAttachmentJsonConverter)
            ?.WallPosts;

        logger.LogInformation("{0}: domain: {1}, name: {2}, count: {3}", nameof(Run), group.Domain, group.Name, posts?.Count ?? 0);

        return posts ?? new ReadOnlyCollection<Post>([]);
    }

    private static void HandlePost(Post post, Group group, List<string> history, List<string> newHistory, Dictionary<string, List<Item>> newContent) {
        // Кроме закреплённых постов
        if (post.IsPinned.HasValue && post.IsPinned.GetValueOrDefault()) {
            return;
        }

        if (!post.OwnerId.HasValue) {
            return;
        }

        if (!post.Id.HasValue) {
            return;
        }

        // Посты с плейлистами
        var attachments = post
            .Attachments
            ?.Where(it => it.Type == typeof(Dto_Link))
            .ToList();

        if (attachments?.Count == 0) {
            return;
        }

        // После обработки названия
        var text = group.Handler.GetPreparedTitle(post);
        if (!text.IsExists) {
            return;
        }

        // Если уже публиковался ранее
        if (history.Contains(text.Name)) {
            return;
        }

        var item = new Item {
            Group = group.Name,
            Name = text.Name,
            Link = $"https://vk.com/wall{post.OwnerId}_{post.Id}",
            Views = post.Views?.Count ?? 0,
            Reactions = post.Likes?.Count ?? 0,
        };

        newHistory.Add(text.Name);
        newContent[group.Name].Add(item);
    }
}
