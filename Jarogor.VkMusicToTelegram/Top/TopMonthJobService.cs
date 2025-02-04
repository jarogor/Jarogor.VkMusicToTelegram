using Microsoft.Extensions.Options;

namespace Jarogor.VkMusicToTelegram.Top;

public sealed class TopMonthJobService(ILogger<TopMonthJobService> logger, IOptions<Options> options) : TopServiceBase(logger, options) {
    protected override TimeSpan Interval() => TimeSpan.FromDays(30);

    protected override async Task Run(CancellationToken stoppingToken) {
        foreach (var group in Constants.VkGroups) {
            Handle(group.Domain, group.Name, group.Handler, group.Top.Month);
        }

        await SendTgMessage("месяц", stoppingToken);
    }
}
