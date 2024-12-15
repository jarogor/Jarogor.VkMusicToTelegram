﻿using Microsoft.Extensions.Options;
using Quartz;

namespace Jarogor.VkMusicToTelegram.Jobs;

[DisallowConcurrentExecution]
public sealed class TopMonthJob(ILogger<TopMonthJob> logger, IOptions<Options> options) : TopJobBase(logger, options) {
    protected override TimeSpan Interval() => TimeSpan.FromDays(30);

    protected override async Task Run(CancellationToken stoppingToken) {
        foreach (var group in Constants.VkGroups) {
            Handle(group.Domain, group.Name, group.Handler, group.Top.Month);
        }

        await SendTgMessage("месяц", stoppingToken);
    }
}
