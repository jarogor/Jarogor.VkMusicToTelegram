using Microsoft.Extensions.Options;

namespace Jarogor.VkMusicToTelegram.Top;

public sealed class TopWeekJobService(ILogger<TopWeekJobService> logger, IOptions<Options> options) : TopServiceBase(logger, options) {
    protected override TimeSpan Interval() => TimeSpan.FromDays(7);

    protected override async Task Run(CancellationToken stoppingToken) {
        foreach (var group in Constants.VkGroups) {
            Handle(group.Domain, group.Name, group.Handler, group.Top.Week);
        }

        await SendTgMessage("неделя", stoppingToken);
    }
}
