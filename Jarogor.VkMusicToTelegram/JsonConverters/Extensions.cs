using Newtonsoft.Json.Linq;
using VkNet.Model;
using VkNet.Utils;
using Link = Jarogor.VkMusicToTelegram.Dto.Link;

namespace Jarogor.VkMusicToTelegram.JsonConverters;

public static class Extensions {
    public static Attachment CustomLinkFromJson(this AttachmentConverterService attachmentConverter, JToken item) {
        var type = item["type"]!.ToString();
        var typeToken = item[type]!;

        return type == "link"
            ? new Attachment {
                Type = typeof(Link),
                Instance = typeToken.ToObject<Link>(),
            }
            : attachmentConverter.FromJson(item);
    }
}
