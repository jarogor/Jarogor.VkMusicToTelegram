using System.Text;
using Jarogor.VkMusicToTelegram.Domain.Vk.Api;

namespace Jarogor.VkMusicToTelegram.Domain.Tg;

public static class ResultExtensions {
    public static string CreateTopMessage(
        this Dictionary<string, List<Result>> items,
        int tgTopCount,
        string topName
    ) {
        var message = new StringBuilder();
        message.AppendLine($"<b>ТОП {tgTopCount} ({topName})</b>");

        foreach (var pair in items) {
            message.AppendLine();
            message.AppendLine(pair.Key);

            var names = pair.Value
                .OrderByDescending(item => item.Reactions)
                .ThenBy(item => item.Views)
                .Take(tgTopCount)
                .ToArray();

            for (var i = 0; i < names.Length; i++) {
                message.AppendLine($"""{i + 1}. <a href="{names[i].Link}">{names[i].Name}</a>""");
            }
        }

        return message.ToString();
    }

    public static string CreateLastMessage(this Dictionary<string, List<Result>> items) {
        var message = new StringBuilder();

        foreach (var pair in items.Where(pair => pair.Value.Count > 0)) {
            message.AppendLine(pair.Key);

            foreach (var item in pair.Value) {
                message.AppendLine($"""- <a href="{item.Link}">{item.Name}</a>""");
            }

            message.AppendLine();
        }

        return message.ToString();
    }

    public static Result CreateItem(
        this Post post,
        string group,
        string entryName
    ) {
        return new Result {
            Group = group,
            Name = entryName,
            Link = $"https://vk.com/wall{post.OwnerId}_{post.Id}",
            Views = post.Views ?? 0,
            Reactions = post.Likes ?? 0,
        };
    }
}
