using Microsoft.Extensions.Logging;

namespace Jarogor.VkMusicToTelegram.Domain.Tg.Top;

public sealed class TopWeekService(ILogger<TopWeekService> logger, TopOptions topOptions) : TopServiceBase(logger, topOptions) {
    private readonly List<Group> _groups = topOptions.Groups;
    protected override string Top => "неделя";
    protected override TimeSpan Interval() => TimeSpan.FromDays(7);

    protected override async Task RunAsync(CancellationToken stoppingToken) {
        foreach (var group in _groups) {
            await HandleAsync(group.Domain, group.Name, group.Parsing, group.Top.Week, stoppingToken);
        }
    }
}
