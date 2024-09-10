using Quartz;
using VkMusicToTelegram;
using VkMusicToTelegram.Jobs;

var builder = Host.CreateDefaultBuilder(args);

var vkApiAccessToken = Environment.GetEnvironmentVariable("VK_TOKEN");
var tgBotId = Environment.GetEnvironmentVariable("TG_BOT_ID");
var tgChannelId = Environment.GetEnvironmentVariable("TG_CHANNEL_ID");

var jobCronLast = Environment.GetEnvironmentVariable("JOB_CRON_LAST") ?? "0 0 */4 * * ?";
var jobCronTop = Environment.GetEnvironmentVariable("JOB_CRON_TOP") ?? "0 0 18 * * ?";

var vkLastCount = Environment.GetEnvironmentVariable("VK_LAST_COUNT") ?? "20";
var vkTopCount = Environment.GetEnvironmentVariable("VK_TOP_COUNT") ?? "30";
var tgTopCount = Environment.GetEnvironmentVariable("TG_TOP_COUNT") ?? "3";

await Console.Out.WriteLineAsync(
    $"""
     VK_TOKEN is set: {!string.IsNullOrWhiteSpace(vkApiAccessToken)}
     TG_BOT_ID:       {tgBotId}
     TG_CHANNEL_ID:   {tgChannelId}
     JOB_CRON_LAST:   "{jobCronLast}"
     JOB_CRON_TOP:    "{jobCronTop}"
     VK_LAST_COUNT:   {vkLastCount}
     VK_TOP_COUNT:    {vkTopCount}
     TG_TOP_COUNT:    {tgTopCount}
     """
);

if (vkApiAccessToken is null) {
    throw new Exception("[VK_TOKEN] environment variable not found.");
}

if (tgBotId is null) {
    throw new Exception("[TG_BOT_ID] environment variable not found.");
}

if (tgChannelId is null) {
    throw new Exception("[TG_CHANNEL_ID] environment variable not found.");
}

builder
    .ConfigureServices((context, services) => {
        services.AddOptions<Options>().Configure(o => {
            o.VkApiAccessToken = vkApiAccessToken;
            o.TgBotId = tgBotId;
            o.TgChannelId = tgChannelId;
            o.VkLastCount = int.Parse(vkLastCount);
            o.VkTopCount = int.Parse(vkTopCount);
            o.TgTopCount = int.Parse(tgTopCount);
        });

        services
            .AddQuartz(q => {
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp => {
                    tp.MaxConcurrency = 10;
                });

                // Крон выражения
                // https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html
                //
                //   Шаблон            Поле           Обязательный    Допустимые значения    Допустимые специальные символы
                //   +———————————————  Секунды        ДА              0-59                   , - * /
                //   | +—————————————  Минуты         ДА              0-59                   , - * /
                //   | | +———————————  Часы           ДА              0-23                   , - * /
                //   | | | +—————————  День месяца    ДА              1-31                   , - * ? / L W
                //   | | | | +———————  Месяцы         ДА              1-12 или JAN-DEC       , - * /
                //   | | | | | +—————  День недели    ДА              1-7 или SUN-SAT        , - * ? / L #
                //   | | | | | | +———  Год            НЕТ             пусто, 1970-2099       , - * /
                //   | | | | | | |
                //   * * * * * ?

                q.ScheduleJob<LastJob>(trigger => trigger.WithCronSchedule(jobCronLast), jobConfigurator => jobConfigurator.DisallowConcurrentExecution());
                q.ScheduleJob<TopJob>(trigger => trigger.WithCronSchedule(jobCronTop), jobConfigurator => jobConfigurator.DisallowConcurrentExecution());
            });

        // Quartz.Extensions.Hosting hosting
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    });

var host = builder.Build();
await host.RunAsync();
