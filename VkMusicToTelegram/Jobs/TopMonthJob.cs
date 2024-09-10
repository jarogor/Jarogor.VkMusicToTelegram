using Microsoft.Extensions.Options;
using Quartz;

namespace VkMusicToTelegram.Jobs;

[DisallowConcurrentExecution]
public sealed class TopMonthJob(ILogger<TopMonthJob> logger, IOptions<Options> options) : TopJobBase(logger, options) {
    protected override TimeSpan Interval() => TimeSpan.FromDays(30);

    protected override async Task Run(CancellationToken stoppingToken) {
        foreach (var group in Constants.VkGroups) {
            Handle(group.domain, group.name, group.handler, group.top.Month);
        }

        await SendTgMessage("месяц", stoppingToken);
    }
}
