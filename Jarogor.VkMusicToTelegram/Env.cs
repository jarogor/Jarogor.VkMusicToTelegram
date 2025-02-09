namespace Jarogor.VkMusicToTelegram;

public static class Env {
    private const string VkTokenKey = "VK_TOKEN";
    private const string TgBotIdKey = "TG_BOT_ID";
    private const string TgChannelIdKey = "TG_CHANNEL_ID";

    private const string VkLastCountKey = "VK_LAST_COUNT";
    private const string VkPostsLimitKey = "VK_POSTS_LIMIT";
    private const string TgTopCountKey = "TG_TOP_COUNT";
    private const string CronLastKey = "JOB_CRON_LAST";
    private const string CronTopWeekKey = "JOB_CRON_TOP_WEEK";
    private const string CronTopMonthKey = "JOB_CRON_TOP_MONTH";

    public static Options Build() {
        return new Options {
            VkApiAccessToken = Environment.GetEnvironmentVariable(VkTokenKey) ?? throw new ArgumentException($"[{VkTokenKey}] environment variable not found."),
            TgBotId = Environment.GetEnvironmentVariable(TgBotIdKey) ?? throw new ArgumentException($"[{TgBotIdKey}] environment variable not found."),
            TgChannelId = Environment.GetEnvironmentVariable(TgChannelIdKey) ?? throw new ArgumentException($"[{TgChannelIdKey}] environment variable not found."),
            VkLastCount = Environment.GetEnvironmentVariable(VkLastCountKey) ?? "20",
            VkPostsLimit = Environment.GetEnvironmentVariable(VkPostsLimitKey) ?? "100",
            TgTopCount = Environment.GetEnvironmentVariable(TgTopCountKey) ?? "5",
            JobCronLast = Environment.GetEnvironmentVariable(CronLastKey) ?? "0 0 */4 * * ?",
            JobCronTopWeek = Environment.GetEnvironmentVariable(CronTopWeekKey) ?? "0 0 18 ? * 6",
            JobCronTopMonth = Environment.GetEnvironmentVariable(CronTopMonthKey) ?? "0 0 20 1 * ?",
        };
    }

    public static async Task PrintHelpAsync(this Options options) {
        await Console.Out.WriteLineAsync(
            $"""
             ----------------------------------------------
             {VkTokenKey} is set: {!string.IsNullOrWhiteSpace(options.VkApiAccessToken)}
             {TgBotIdKey}: {options.TgBotId}
             {TgChannelIdKey}: {options.TgChannelId}
             {CronLastKey}: "{options.JobCronLast}"
             {CronTopWeekKey}: "{options.JobCronTopWeek}"
             {CronTopMonthKey}: "{options.JobCronTopMonth}"
             {VkLastCountKey}: {options.VkLastCount}
             {VkPostsLimitKey}: {options.VkPostsLimit}
             {TgTopCountKey}: {options.TgTopCount}
             ----------------------------------------------
             """
        );
    }

    public sealed class Options {
        public string VkApiAccessToken { get; init; } = null!;
        public string TgBotId { get; init; } = null!;
        public string TgChannelId { get; init; } = null!;
        public string VkLastCount { get; init; } = null!;
        public string VkPostsLimit { get; init; } = null!;
        public string TgTopCount { get; init; } = null!;
        public string JobCronLast { get; init; } = null!;
        public string JobCronTopWeek { get; init; } = null!;
        public string JobCronTopMonth { get; init; } = null!;
    }
}
