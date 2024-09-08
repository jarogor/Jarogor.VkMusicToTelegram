using System.Text;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using VkMusicToTelegram.Dto;
using VkMusicToTelegram.PostHandlers;
using VkMusicToTelegram.PostHandlers.Handlers;
using VkNet;
using VkNet.Model;
using VkNet.Utils;
using Link = VkMusicToTelegram.Dto.Link;

namespace VkMusicToTelegram;

public class VkMusicToTelegramWorker(IOptions<Options> options) : BackgroundService
{
    // TODO
    // - нужен Qartz наверное, то есть регулярный гибкий запуск в какое-то время,
    //      либо список времени когда запускать и самому сделать воркер
    // - нужно хранлище, быть может лучше в файле, чтобы запрашивать последние 10 записей,
    //      например, и проверять, есть ли они уже или ещё нет,
    //      быть может завести sqlite базу (скорее всего) для этого, потому что нужно как-то записывать показатели
    //      и рассчитывать их затем, чтобы публиковать только топ 5, например
    //      и при этом пропускать уже полученные 
    private readonly List<(string domain, string name, IHandler handler)> _groups =
    [
        ("blackwall", "Blackwall", new BlackWall()),
        ("asylumforesters_vk", "Убежище Лесников", new AsylumForesters()),
        ("black_metal_promotion", "Black Metal Promotion", new BlackMetalPromotion()),
        // ("bestblackmetal", "BEST BLACK METAL"),
        ("e_black_metal", @"E:\music\black metal", new EBlackMetal()),
        ("ru_black_metal", "Русский блэк-метал", new RuBlackMetal())
    ];

    private readonly VkApi _vkApiClient = new();
    private readonly ApiAuthParams _vkApiAuthParams = new() { AccessToken = options.Value.VkApiAccessToken };
    private readonly int _vkCountPosts = options.Value.VkCountPosts;
    private readonly TelegramBotClient _tgApiClient = new(options.Value.TgBotId);
    private readonly string _tgChannelId = options.Value.TgChannelId;
    private readonly double _jobIntervalHours = options.Value.JobIntervalHours;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Run();
            await Task.Delay(TimeSpan.FromHours(_jobIntervalHours), stoppingToken);
        }
    }

    private void Run()
    {
        _vkApiClient.AuthorizeAsync(_vkApiAuthParams).GetAwaiter().GetResult();
        var data = new Dictionary<string, List<string>>();

        foreach (var group in _groups)
        {
            var vkParameters = new VkParameters
            {
                { "domain", group.domain },
                { "count", _vkCountPosts },
            };

            var posts = _vkApiClient.Call<CustomWall>("wall.get", vkParameters, false, new CustomAttachmentJsonConverter());

            foreach (var post in posts.WallPosts)
            {
                // Кроме закреплённых постов
                if (post.IsPinned.HasValue && post.IsPinned.GetValueOrDefault())
                {
                    continue;
                }

                // Посты с плейлистами
                var attachments = post.Attachments
                    .Where(it => it.Type == typeof(Link))
                    .ToList();
                if (attachments.Count == 0)
                {
                    continue;
                }

                // После обработки названия
                var item = group.handler.GetPreparedTitle(post);
                if (!item.IsExists)
                {
                    continue;
                }

                // Группировка по названиям пабликов
                if (!data.ContainsKey(group.name))
                {
                    data[group.name] = [];
                }

                // TODO оформить в отдельный форматер, а не раскидывать по частям
                data[group.name].Add($"[{item.Name}](https://vk.com/wall{post.OwnerId}_{post.Id})");
            }
        }

        // Формирование сообщения
        var message = new StringBuilder();
        foreach (var pair in data)
        {
            message.AppendLine($"__{pair.Key}__");
            message.AppendLine("- " + string.Join("\n- ", pair.Value));
            message.AppendLine();
        }

        // Отправка в Телеграм
        _tgApiClient
            .SendTextMessageAsync(_tgChannelId, message.ToString(), parseMode: ParseMode.Markdown, disableWebPagePreview: true)
            .GetAwaiter()
            .GetResult();
    }
}