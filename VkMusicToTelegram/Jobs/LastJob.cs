using System.Text;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using VkMusicToTelegram.Dto;
using VkNet;
using VkNet.Model;
using VkNet.Utils;
using Link = VkMusicToTelegram.Dto.Link;

namespace VkMusicToTelegram.Jobs;

[DisallowConcurrentExecution]
public sealed class LastJob(ILogger<LastJob> logger, IOptions<Options> options) : IJob {
    private readonly VkApi _vkApiClient = new();
    private readonly ApiAuthParams _vkApiAuthParams = new() { AccessToken = options.Value.VkApiAccessToken };
    private readonly TelegramBotClient _tgApiClient = new(options.Value.TgBotId);
    private readonly string _tgChannelId = options.Value.TgChannelId;

    private readonly int _vkLastCount = options.Value.VkLastCount;
    private readonly string _historyListFilePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "history-list.txt");

    public Task Execute(IJobExecutionContext context)
        => Run(context.CancellationToken);

    private async Task Run(CancellationToken stoppingToken) {
        await _vkApiClient.AuthorizeAsync(_vkApiAuthParams, stoppingToken);

        var newContent = Constants.VkGroups.ToDictionary(group => group.name, _ => new List<Item>());
        var newHistory = new List<string>();
        var history = File.Exists(_historyListFilePath) 
            ? (await File.ReadAllLinesAsync(_historyListFilePath, stoppingToken)).ToList()
            : [];

        foreach (var group in Constants.VkGroups) {
            var vkParameters = new VkParameters {
                { "domain", group.domain },
                { "count", _vkLastCount },
            };

            var posts = _vkApiClient.Call<CustomWall>("wall.get", vkParameters, false, Constants.CustomAttachmentJsonConverter);
            logger.LogInformation("{0}: domain: {1}, name: {2}, count: {3}", nameof(Run), group.domain, group.name, posts.TotalCount);

            foreach (var post in posts.WallPosts) {
                // Кроме закреплённых постов
                if (post.IsPinned.HasValue && post.IsPinned.GetValueOrDefault()) {
                    continue;
                }

                // Посты с плейлистами
                var attachments = post.Attachments
                    .Where(it => it.Type == typeof(Link))
                    .ToList();
                if (attachments.Count == 0) {
                    continue;
                }

                // После обработки названия
                var item = group.handler.GetPreparedTitle(post);
                if (!item.IsExists) {
                    continue;
                }

                // Если уже публиковался ранее
                if (history.Contains(item.Name)) {
                    continue;
                }

                newHistory.Add(item.Name);
                newContent[group.name].Add(new Item {
                    Group = group.name,
                    Name = item.Name,
                    Link = $"https://vk.com/wall{post.OwnerId}_{post.Id}",
                    Views = post.Views.Count,
                    Reactions = post.Likes.Count,
                });
            }
        }

        if (newHistory.Count <= 0) {
            return;
        }

        // Формирование сообщения
        var message = new StringBuilder();
        var items = newContent.Where(pair => pair.Value.Count > 0);
        foreach (var pair in items) {
            message.AppendLine(pair.Key);
            foreach (var item in pair.Value) {
                message.AppendLine("- " + $"[{item.Name}]({item.Link})");
            }

            message.AppendLine();
        }

        logger.LogInformation("new items count: {0}", newHistory.Count);

        // Отправка в Телеграм
        await _tgApiClient.SendTextMessageAsync(
            _tgChannelId,
            message.ToString(),
            parseMode: ParseMode.Markdown,
            disableWebPagePreview: true,
            cancellationToken: stoppingToken
        );

        // Добавление новых записей в историю
        history.AddRange(newHistory);
        await File.WriteAllLinesAsync(_historyListFilePath, history, stoppingToken);
    }
}
