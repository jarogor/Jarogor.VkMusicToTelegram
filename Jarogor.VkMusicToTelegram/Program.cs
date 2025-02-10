using Jarogor.VkMusicToTelegram;
using Jarogor.VkMusicToTelegram.Domain;
using Jarogor.VkMusicToTelegram.Domain.Tg.Last;
using Jarogor.VkMusicToTelegram.Domain.Tg.Top;
using Jarogor.VkMusicToTelegram.Jobs;
using Quartz;
using IVkAdapter = Jarogor.VkMusicToTelegram.Domain.Vk.Api.IAdapter;
using VkAdapter = Jarogor.VkMusicToTelegram.Infrastructure.Vk.Adapter;
using ITgAdapter = Jarogor.VkMusicToTelegram.Domain.Tg.IAdapter;
using TgAdapter = Jarogor.VkMusicToTelegram.Infrastructure.Tg.Adapter;

var env = Env.Build();

await env.PrintHelpAsync();

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => {
        services.AddTransient<IVkAdapter, VkAdapter>(_ => new VkAdapter(env.VkApiAccessToken));
        services.AddTransient<ITgAdapter, TgAdapter>(_ => new TgAdapter(env.TgChannelId, env.TgBotId));

        services.AddTransient<LastOptions>(it => new LastOptions {
            VkPostsCount = int.Parse(env.VkLastCount),
            TgAdapter = it.GetRequiredService<ITgAdapter>(),
            VkAdapter = it.GetRequiredService<IVkAdapter>(),
            Groups = Constants.VkGroupsSettings,
        });

        services.AddTransient<TopOptions>(it => new TopOptions {
            VkPostsLimit = int.Parse(env.VkPostsLimit),
            TgTopCount = int.Parse(env.TgTopCount),
            TgAdapter = it.GetRequiredService<ITgAdapter>(),
            VkAdapter = it.GetRequiredService<IVkAdapter>(),
            Groups = Constants.VkGroupsSettings,
        });

        services.AddTransient<LastService>();
        services.AddTransient<TopWeekService>();
        services.AddTransient<TopMonthService>();

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

                q.ScheduleJob<LastJob>(trigger => trigger.WithCronSchedule(env.JobCronLast));
                q.ScheduleJob<TopWeekJob>(trigger => trigger.WithCronSchedule(env.JobCronTopWeek));
                q.ScheduleJob<TopMonthJob>(trigger => trigger.WithCronSchedule(env.JobCronTopMonth));
            });

        // Quartz.Extensions.Hosting hosting
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    })
    .Build();

await host.RunAsync();
