using Jarogor.VkMusicToTelegram.Domain.Tg.Top;
using Quartz;

namespace Jarogor.VkMusicToTelegram.Jobs;

[DisallowConcurrentExecution]
public sealed class TopMonthJob(TopMonthService topService) : IJob {
    public async Task Execute(IJobExecutionContext context)
        => await topService.ExecuteAsync(context.CancellationToken);
}
