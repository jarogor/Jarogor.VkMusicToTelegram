using Quartz;
using VkMusicToTelegram;
using VkMusicToTelegram.Jobs;

var builder = Host.CreateDefaultBuilder(args);

var vkApiAccessToken = Environment.GetEnvironmentVariable("VK_API_ACCESS_TOKEN");
var tgBotId = Environment.GetEnvironmentVariable("TG_BOT_ID");
var tgChannelId = Environment.GetEnvironmentVariable("TG_CHANNEL_ID");
var vkCountPosts = Environment.GetEnvironmentVariable("VK_COUNT_POSTS") ?? "10";
var tgTopPublicationCount = Environment.GetEnvironmentVariable("TOP_COUNT") ?? "20";
var jobLatestCron = Environment.GetEnvironmentVariable("JOB_LATEST_CRON") ?? "0 0 18 * * ?";
var jobTopCron = Environment.GetEnvironmentVariable("JOB_TOP_CRON") ?? "0 0 */4 * * ?";

await Console.Out.WriteLineAsync(
    $"""
     VK_API_ACCESS_TOKEN is empty: {string.IsNullOrWhiteSpace(vkApiAccessToken)}
     TG_BOT_ID:       [{tgBotId}]
     TG_CHANNEL_ID:   [{tgChannelId}]
     VK_COUNT_POSTS:  [{vkCountPosts}]
     TOP_COUNT:       [{tgTopPublicationCount}]
     JOB_LATEST_CRON: [{jobLatestCron}]
     JOB_TOP_CRON:    [{jobTopCron}]
     """
);

builder
    .ConfigureServices((context, services) => {
        services.AddOptions<Options>().Configure(o => {
            if (vkApiAccessToken is null) {
                throw new Exception("[VK_API_ACCESS_TOKEN] environment variable not found.");
            }

            if (tgBotId is null) {
                throw new Exception("[TG_BOT_ID] environment variable not found.");
            }

            if (tgChannelId is null) {
                throw new Exception("[TG_CHANNEL_ID] environment variable not found.");
            }

            o.VkCountPosts = int.Parse(vkCountPosts);
            o.TopCount = int.Parse(tgTopPublicationCount);
            o.TgBotId = tgBotId;
            o.TgChannelId = tgChannelId;
            o.VkApiAccessToken = vkApiAccessToken;
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

                q.ScheduleJob<LatestJob>(trigger => trigger.WithCronSchedule(jobLatestCron));
                q.ScheduleJob<TopJob>(trigger => trigger.WithCronSchedule(jobTopCron));
            });

        // Quartz.Extensions.Hosting hosting
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    });

var host = builder.Build();
await host.RunAsync();
