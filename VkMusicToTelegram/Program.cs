using Quartz;
using VkMusicToTelegram;
using VkMusicToTelegram.Jobs;

const string vkToken = "VK_TOKEN";
const string botId = "TG_BOT_ID";
const string channelId = "TG_CHANNEL_ID";
const string cronLast = "JOB_CRON_LAST";
const string cronTopWeek = "JOB_CRON_TOP_WEEK";
const string cronTopMonth = "JOB_CRON_TOP_MONTH";
const string lastCount = "VK_LAST_COUNT";
const string topCount = "TG_TOP_COUNT";

var vkApiAccessToken = Environment.GetEnvironmentVariable(vkToken);
var tgBotId = Environment.GetEnvironmentVariable(botId);
var tgChannelId = Environment.GetEnvironmentVariable(channelId);

var jobCronLast = Environment.GetEnvironmentVariable(cronLast) ?? "0 0 */4 * * ?";
var jobCronTopWeek = Environment.GetEnvironmentVariable(cronTopWeek) ?? "0 0 18 ? * 6";
var jobCronTopMonth = Environment.GetEnvironmentVariable(cronTopMonth) ?? "0 0 20 1 * ?";

var vkLastCount = Environment.GetEnvironmentVariable(lastCount) ?? "20";
var tgTopCount = Environment.GetEnvironmentVariable(topCount) ?? "5";

await Console.Out.WriteLineAsync(
    $"""
     ----------------------------------------------
     {vkToken} is set: {!string.IsNullOrWhiteSpace(vkApiAccessToken)}
     {botId}:          {tgBotId}
     {channelId}:      {tgChannelId}

     {cronLast}:     "{jobCronLast}"
     {cronTopWeek}:  "{jobCronTopWeek}"
     {cronTopMonth}: "{jobCronTopMonth}"

     {lastCount}: {vkLastCount}
     {topCount}:  {tgTopCount}
     ----------------------------------------------
     """
);

if (vkApiAccessToken is null) {
    throw new Exception($"[{vkToken}] environment variable not found.");
}

if (tgBotId is null) {
    throw new Exception($"[{botId}] environment variable not found.");
}

if (tgChannelId is null) {
    throw new Exception($"[{channelId}] environment variable not found.");
}


var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => {
        services.AddOptions<Options>().Configure(o => {
            o.VkApiAccessToken = vkApiAccessToken;
            o.TgBotId = tgBotId;
            o.TgChannelId = tgChannelId;
            o.VkLastCount = int.Parse(vkLastCount);
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

                q.ScheduleJob<LastJob>(trigger => trigger.WithCronSchedule(jobCronLast));
                q.ScheduleJob<TopWeekJob>(trigger => trigger.WithCronSchedule(jobCronTopWeek));
                q.ScheduleJob<TopMonthJob>(trigger => trigger.WithCronSchedule(jobCronTopMonth));
            });

        // Quartz.Extensions.Hosting hosting
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    })
    .Build();

await host.RunAsync();
