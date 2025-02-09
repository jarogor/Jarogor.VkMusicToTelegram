using Jarogor.VkMusicToTelegram.Domain.Tg;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Tg;

public sealed class Adapter(string tgChannelId, string tgBotId) : IAdapter {
    private readonly TelegramBotClient _tgApiClient = new(tgBotId);

    public async Task SendMessage(string message, CancellationToken stoppingToken) {
        await _tgApiClient
            .SendMessage(
                tgChannelId,
                message,
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
                parseMode: ParseMode.Html,
                cancellationToken: stoppingToken
            );
    }

    public async Task SendMessage(Dictionary<string, List<Result>> items, CancellationToken stoppingToken) {
        await Send(items, stoppingToken);
    }

    private async Task Send(Dictionary<string, List<Result>> items, CancellationToken stoppingToken) {
        try {
            await SendMessage(items.CreateLastMessage(), stoppingToken);
        } catch (RequestException) {
            //  Telegram Bot API error 400: Bad Request: message is too long
            var limit = items.Values.Sum(it => it.Count) / 2;
            var (first, second) = Split(items, limit);
            await Send(first, stoppingToken);
            await Send(second, stoppingToken);
        }
    }

    private static (Dictionary<string, List<Result>> first, Dictionary<string, List<Result>> second) Split(Dictionary<string, List<Result>> items, int limit) {
        var first = new Dictionary<string, List<Result>>();
        var second = new Dictionary<string, List<Result>>();

        foreach (var (key, list) in items) {
            first[key] = list.Take(limit).ToList();

            var results = list.Skip(limit).ToList();
            if (results.Count > 0) {
                second[key] = results;
            }
        }

        return (first, second);
    }
}
