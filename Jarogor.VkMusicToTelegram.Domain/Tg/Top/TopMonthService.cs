using Microsoft.Extensions.Logging;

namespace Jarogor.VkMusicToTelegram.Domain.Tg.Top;

public sealed class TopMonthService(ILogger<TopMonthService> logger, TopOptions topOptions) : TopServiceBase(logger, topOptions) {
    private readonly List<Group> _groups = topOptions.Groups;
    protected override string Top => "месяц";
    protected override TimeSpan Interval() => TimeSpan.FromDays(30);

    protected override async Task RunAsync(CancellationToken stoppingToken) {
        foreach (var group in _groups) {
            await HandleAsync(group.Domain, group.Name, group.Parsing, group.Top.Month, stoppingToken);
        }
    }
}
