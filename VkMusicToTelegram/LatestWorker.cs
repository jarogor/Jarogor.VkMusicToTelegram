using System.Text;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using VkMusicToTelegram.Dto;
using VkNet;
using VkNet.Model;
using VkNet.Utils;
using Link = VkMusicToTelegram.Dto.Link;

namespace VkMusicToTelegram;

public class LatestWorker(ILogger<LatestWorker> logger, IOptions<Options> options) : BackgroundService {
    private readonly VkApi _vkApiClient = new();
    private readonly ApiAuthParams _vkApiAuthParams = new() { AccessToken = options.Value.VkApiAccessToken };
    private readonly int _vkCountPosts = options.Value.VkCountPosts;
    private readonly TelegramBotClient _tgApiClient = new(options.Value.TgBotId);
    private readonly string _tgChannelId = options.Value.TgChannelId;
    private readonly double _jobIntervalHours = options.Value.JobIntervalHours;
    private readonly string _historyListFilePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "history-list.txt");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            logger.LogInformation("run latest");
            Run();
            await Task.Delay(TimeSpan.FromHours(_jobIntervalHours), stoppingToken);
        }
    }

    private void Run() {
        _vkApiClient.AuthorizeAsync(_vkApiAuthParams).GetAwaiter().GetResult();
        var newContent = new Dictionary<string, List<TopItem>>();
        var history = File.Exists(_historyListFilePath) ? File.ReadAllLines(_historyListFilePath).ToList() : [];
        var newHistory = new List<string>();

        foreach (var group in Constants.VkGroups) {
            var vkParameters = new VkParameters {
                { "domain", group.domain },
                { "count", _vkCountPosts },
            };

            var posts = _vkApiClient.Call<CustomWall>("wall.get", vkParameters, false, Constants.CustomAttachmentJsonConverter);

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

                // Группировка по названиям пабликов
                if (!newContent.ContainsKey(group.name)) {
                    newContent[group.name] = [];
                }

                // Если уже публиковался ранее
                if (history.Contains(item.Name)) {
                    continue;
                }

                newHistory.Add(item.Name);
                newContent[group.name].Add(new TopItem {
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
        _tgApiClient
            .SendTextMessageAsync(_tgChannelId, message.ToString(), parseMode: ParseMode.Markdown, disableWebPagePreview: true)
            .GetAwaiter()
            .GetResult();

        // Добавление новых записей в историю
        history.AddRange(newHistory);
        File.WriteAllLines(_historyListFilePath, history);
    }
}
