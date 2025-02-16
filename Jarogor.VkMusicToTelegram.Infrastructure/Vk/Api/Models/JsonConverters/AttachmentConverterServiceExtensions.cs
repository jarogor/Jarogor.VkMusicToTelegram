using Newtonsoft.Json.Linq;
using VkNet.Model;
using VkNet.Utils;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models.JsonConverters;

public static class AttachmentConverterServiceExtensions {
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
