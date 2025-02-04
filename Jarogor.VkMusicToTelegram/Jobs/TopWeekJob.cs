using Jarogor.VkMusicToTelegram.Top;
using Quartz;

namespace Jarogor.VkMusicToTelegram.Jobs;

[DisallowConcurrentExecution]
public sealed class TopWeekJob(TopWeekJobService topWeekJobService) : TopJobBase(topWeekJobService);
