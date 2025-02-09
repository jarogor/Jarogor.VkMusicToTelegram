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
            await SendMessage(items.CreateMessage(), stoppingToken);
        } catch (RequestException) {
            // Telegram Bot API error 400: Bad Request: message is too long
            // Сначала нужно разделить общее количество на количество списков,
            // чтобы получить среднее количество элементов списка.
            // Затем ещё пополам, чтобы его разбить на две части для разной отправки.
            var limit = items.Sum(it => it.Value.Count) / items.Count / 2;
            var (first, second) = items.Split(limit);
            await Send(first, stoppingToken);
            await Send(second, stoppingToken);
        }
    }
}
