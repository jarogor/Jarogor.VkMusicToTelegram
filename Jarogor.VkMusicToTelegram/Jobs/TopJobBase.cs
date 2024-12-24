using System.Collections.ObjectModel;
using System.Text;
using Jarogor.VkMusicToTelegram.Dto;
using Jarogor.VkMusicToTelegram.PostHandlers;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VkNet;
using VkNet.Model;
using VkNet.Utils;
using Dto_Link = Jarogor.VkMusicToTelegram.Dto.Link;
using Dto_Post = Jarogor.VkMusicToTelegram.Dto.Post;

namespace Jarogor.VkMusicToTelegram.Jobs;

public abstract class TopJobBase(ILogger<TopJobBase> logger, IOptions<Options> options) : IJob {
    private readonly TelegramBotClient _tgApiClient = new(options.Value.TgBotId);
    private readonly string _tgChannelId = options.Value.TgChannelId;

    private readonly int _tgTopCount = options.Value.TgTopCount;
    private readonly Dictionary<string, List<Item>> _topContent = new();
    private readonly ApiAuthParams _vkApiAuthParams = new() { AccessToken = options.Value.VkApiAccessToken };

    private readonly VkApi _vkApiClient = new();

    public async Task Execute(IJobExecutionContext context) {
        _vkApiClient.Authorize(_vkApiAuthParams);
        await Run(context.CancellationToken);
    }

    protected abstract TimeSpan Interval();
    protected abstract Task Run(CancellationToken stoppingToken);

    protected void Handle(string domain, string groupName, IHandler handler, int count) {
        logger.LogInformation("{0}: domain: {1}, name: {2}, count: {3}", nameof(Handle), domain, groupName, count);

        foreach (var post in GetPosts(domain, count)) {
            if (IsSkip(post)) {
                continue;
            }

            var item = handler.GetPreparedTitle(post);
            if (!item.IsExists) {
                continue;
            }

            Append(groupName, item, post);
        }
    }

    // Добавление записи в итоговую коллекцию
    private void Append(string groupName, Record record, Dto_Post post) {
        var item = new Item {
            Group = groupName,
            Name = record.Name,
            Link = $"https://vk.com/wall{post.OwnerId}_{post.Id}",
            Views = post.Views?.Count ?? 0,
            Reactions = post.Likes?.Count ?? 0,
        };

        if (!_topContent.ContainsKey(groupName)) {
            logger.LogInformation("{0} _topContent groupName: {1}", nameof(Append), groupName);
            _topContent[groupName] = [];
        }

        _topContent[groupName].Add(item);
    }

    private static bool IsSkip(Dto_Post post) {
        // Пропуск закрепленных постов
        if (post.IsPinned.HasValue && post.IsPinned.GetValueOrDefault()) {
            return true;
        }

        if (!post.OwnerId.HasValue) {
            return true;
        }

        if (!post.Id.HasValue) {
            return true;
        }

        var attachments = post.Attachments
            .Where(it => it.Type == typeof(Dto_Link))
            .ToList();

        // Пропуск постов без вложений
        return attachments.Count == 0;
    }

    protected async Task SendTgMessage(string topName, CancellationToken stoppingToken) {
        // Пропуск, если нет новых
        if (_topContent.Count <= 0) {
            return;
        }

        logger.LogInformation("{0}, topName {1}", nameof(SendTgMessage), topName);

        // Отправка в Телеграм
        await _tgApiClient
            .SendMessage(
                _tgChannelId,
                CreateMessage(topName),
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                parseMode: ParseMode.Html,
                cancellationToken: stoppingToken
            );
    }

    private string CreateMessage(string topName) {
        var message = new StringBuilder();
        message.AppendLine($"**ТОП {_tgTopCount} ({topName})**");

        foreach (var pair in _topContent) {
            message.AppendLine();
            message.AppendLine(pair.Key);

            var names = pair.Value
                .OrderByDescending(it => it.Reactions)
                .ThenBy(it => it.Views)
                .Select(it => it)
                .Take(_tgTopCount)
                .ToArray();

            for (var i = 0; i < names.Length; i++) {
                message.AppendLine($"""{i + 1}. <a href="{names[i].Link}">{names[i].Name}</a>""");
            }
        }

        logger.LogInformation("{0}", message);

        return message.ToString();
    }

    private List<Dto_Post> GetPosts(string domain, int count) {
        if (count <= Constants.VkRequestLimit) {
            return CreateIntervalPosts(
                VkRequest(domain, count, 0)
                    .OrderByDescending(it => it.Date)
                    .ToList()
            );
        }

        var posts = new List<Dto_Post>();

        while (true) {
            posts.AddRange(VkRequest(domain, Constants.VkRequestLimit, posts.Count).ToList());
            if (posts.Count >= count) {
                break;
            }
        }

        return CreateIntervalPosts(posts);
    }

    private List<Dto_Post> CreateIntervalPosts(List<Dto_Post> posts) {
        var orderByDescending = posts
            .OrderByDescending(it => it.Date)
            .ToList();

        var latestDate = orderByDescending.First().Date;
        var interval = Interval();

        logger.LogInformation("{0}, latestDate: {1}, interval: {2}", nameof(CreateIntervalPosts), latestDate, interval);

        return orderByDescending
            .TakeWhile(it => latestDate - it.Date < interval)
            .ToList();
    }

    private ReadOnlyCollection<Dto_Post> VkRequest(string domain, int count, int offset) {
        logger.LogInformation("{0}, domain: {1}, count: {2}, offset: {3}", nameof(VkRequest), domain, count, offset);

        return _vkApiClient
            .Call<CustomWall>("wall.get", new VkParameters {
                { "domain", domain },
                { "offset", offset },
                { "count", count },
            }, false, Constants.CustomAttachmentJsonConverter)
            .WallPosts;
    }
}
