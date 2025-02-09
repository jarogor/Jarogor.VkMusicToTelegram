using Jarogor.VkMusicToTelegram.Domain.Tg.Last;
using Quartz;

namespace Jarogor.VkMusicToTelegram.Jobs;

[DisallowConcurrentExecution]
public sealed class LastJob(LastService service) : IJob {
    public Task Execute(IJobExecutionContext context)
        => service.RunAsync(context.CancellationToken);
}
