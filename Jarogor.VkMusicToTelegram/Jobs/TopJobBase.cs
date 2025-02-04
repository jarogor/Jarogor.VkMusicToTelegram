using Jarogor.VkMusicToTelegram.Top;
using Quartz;

namespace Jarogor.VkMusicToTelegram.Jobs;

public abstract class TopJobBase(TopServiceBase topService) : IJob {
    public async Task Execute(IJobExecutionContext context)
        => await topService.ExecuteAsync(context.CancellationToken);
}
