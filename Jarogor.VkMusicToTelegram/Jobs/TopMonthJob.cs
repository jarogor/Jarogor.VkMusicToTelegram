using Jarogor.VkMusicToTelegram.Top;
using Quartz;

namespace Jarogor.VkMusicToTelegram.Jobs;

[DisallowConcurrentExecution]
public sealed class TopMonthJob(TopMonthJobService topMonthJobService) : TopJobBase(topMonthJobService);
