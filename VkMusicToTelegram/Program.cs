using VkMusicToTelegram;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<LatestWorker>();
builder.Services.AddHostedService<TopWorker>();

builder.Services.AddOptions<Options>().Configure(o =>
{
    var vkApiAccessToken = Environment.GetEnvironmentVariable("VK_API_ACCESS_TOKEN");
    if (vkApiAccessToken is null)
    {
        throw new Exception("[VK_API_ACCESS_TOKEN] environment variable not found.");
    }

    var tgBotId = Environment.GetEnvironmentVariable("TG_BOT_ID");
    if (tgBotId is null)
    {
        throw new Exception("[TG_BOT_ID] environment variable not found.");
    }

    var tgChannelId = Environment.GetEnvironmentVariable("TG_CHANNEL_ID");
    if (tgChannelId is null)
    {
        throw new Exception("[TG_CHANNEL_ID] environment variable not found.");
    }

    o.JobIntervalHours = int.Parse(Environment.GetEnvironmentVariable("JOB_INTERVAL_HOURS") ?? "4");
    o.VkCountPosts = int.Parse(Environment.GetEnvironmentVariable("VK_COUNT_POSTS") ?? "10");
    o.TopCount = int.Parse(Environment.GetEnvironmentVariable("TOP_COUNT") ?? "20");
    o.TgBotId = tgBotId;
    o.TgChannelId = tgChannelId;
    o.VkApiAccessToken = vkApiAccessToken;
});

var host = builder.Build();
await host.RunAsync();
