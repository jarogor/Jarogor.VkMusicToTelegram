﻿using System.Collections.ObjectModel;
using System.Text;
using Jarogor.VkMusicToTelegram.Jobs;
using Jarogor.VkMusicToTelegram.Vk.Api;
using Jarogor.VkMusicToTelegram.Vk.Posts;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VkNet;
using VkNet.Model;
using VkNet.Utils;
using Link = Jarogor.VkMusicToTelegram.Vk.Api.Link;
using Post = Jarogor.VkMusicToTelegram.Vk.Api.Post;

namespace Jarogor.VkMusicToTelegram.Tg;

public abstract class TopServiceBase(ILogger<TopServiceBase> logger, IOptions<Options> options) {
    private readonly TelegramBotClient _tgApiClient = new(options.Value.TgBotId);
    private readonly string _tgChannelId = options.Value.TgChannelId;

    private readonly int _tgTopCount = options.Value.TgTopCount;
    private readonly Dictionary<string, List<Item>> _topContent = new();

    private readonly ApiAuthParams _vkApiAuthParams = new() { AccessToken = options.Value.VkApiAccessToken };

    private readonly VkApi _vkApiClient = new();

    protected abstract TimeSpan Interval();
    protected abstract Task Run(CancellationToken stoppingToken);

    public async Task ExecuteAsync(CancellationToken cancellationToken) {
        await _vkApiClient.AuthorizeAsync(_vkApiAuthParams, cancellationToken);
        await Run(cancellationToken);
    }

    protected void Handle(string domain, string groupName, IParsing parsing, int count) {
        logger.LogInformation("{0}: domain: {1}, name: {2}, count: {3}", nameof(Handle), domain, groupName, count);

        foreach (var post in GetPosts(domain, count)) {
            if (IsSkip(post)) {
                continue;
            }

            var item = parsing.GetPreparedTitle(post);
            if (!item.IsExists) {
                continue;
            }

            Append(groupName, item, post);
        }
    }

    // Добавление записи в итоговую коллекцию
    private void Append(string groupName, Record record, Post post) {
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

    private static bool IsSkip(Post post) {
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

        if (post.Attachments is null) {
            return false;
        }

        var attachments = post.Attachments
            .Where(it => it.Type == typeof(Link))
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
        message.AppendLine($"<b>ТОП {_tgTopCount} ({topName})</b>");

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

    private List<Post> GetPosts(string domain, int count) {
        if (count <= Constants.VkRequestLimit) {
            return CreateIntervalPosts(
                VkRequest(domain, count, 0)
                    ?.OrderByDescending(it => it.Date)
                    .ToList() ?? []
            );
        }

        var posts = new List<Post>();

        while (true) {
            posts.AddRange(VkRequest(domain, Constants.VkRequestLimit, posts.Count)?.ToList() ?? []);
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

        logger.LogInformation("{0}, latestDate: {1}, interval: {2}", nameof(CreateIntervalPosts), latestDate, interval);

        return orderByDescending
            .TakeWhile(it => latestDate - it.Date < interval)
            .ToList();
    }

    private ReadOnlyCollection<Post>? VkRequest(string domain, int count, int offset) {
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
