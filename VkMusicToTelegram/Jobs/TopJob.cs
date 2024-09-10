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

public class TopJob(IOptions<Options> options) : IJob {
    private readonly VkApi _vkApiClient = new();
    private readonly ApiAuthParams _vkApiAuthParams = new() { AccessToken = options.Value.VkApiAccessToken };
    private readonly TelegramBotClient _tgApiClient = new(options.Value.TgBotId);
    private readonly string _tgChannelId = options.Value.TgChannelId;

    private readonly int _vkTopCount = options.Value.VkTopCount;
    private readonly int _tgTopCount = options.Value.TgTopCount;

    public Task Execute(IJobExecutionContext context)
        => Run(context.CancellationToken);

    private async Task Run(CancellationToken stoppingToken) {
        await _vkApiClient.AuthorizeAsync(_vkApiAuthParams, stoppingToken);
        var topContent = new Dictionary<string, List<Item>>();

        foreach (var group in Constants.VkGroups) {
            var vkParameters = new VkParameters {
                { "domain", group.domain },
                { "count", _vkTopCount },
            };
            var posts = _vkApiClient.Call<CustomWall>("wall.get", vkParameters, false, Constants.CustomAttachmentJsonConverter);

            foreach (var post in posts.WallPosts) {
                if (post.IsPinned.HasValue && post.IsPinned.GetValueOrDefault()) {
                    continue;
                }

                var attachments = post.Attachments
                    .Where(it => it.Type == typeof(Link))
                    .ToList();
                if (attachments.Count == 0) {
                    continue;
                }

                var item = group.handler.GetPreparedTitle(post);
                if (!item.IsExists) {
                    continue;
                }

                if (!topContent.ContainsKey(group.name)) {
                    topContent[group.name] = new List<Item>();
                }

                topContent[group.name].Add(new Item {
                    Group = group.name,
                    Name = item.Name,
                    Link = $"https://vk.com/wall{post.OwnerId}_{post.Id}",
                    Views = post.Views.Count,
                    Reactions = post.Likes.Count,
                });
            }
        }

        if (topContent.Count <= 0) {
            return;
        }

        var message = new StringBuilder();
        message.AppendLine($"**ТОП {_tgTopCount} (из {_vkTopCount})**");

        foreach (var pair in topContent) {
            message.AppendLine();
            message.AppendLine(pair.Key);

            var names = pair.Value
                .OrderByDescending(it => it.Reactions)
                .ThenBy(it => it.Views)
                .Select(it => it)
                .Take(_tgTopCount)
                .ToArray();

            for (int i = 0; i < names.Length; i++) {
                message.AppendLine($"{i + 1}. [{names[i].Name}]({names[i].Link})");
            }
        }

        // Отправка в Телеграм
        await _tgApiClient.SendTextMessageAsync(
            _tgChannelId,
            message.ToString(),
            parseMode: ParseMode.Markdown,
            disableWebPagePreview: true,
            cancellationToken: stoppingToken
        );
    }
}
