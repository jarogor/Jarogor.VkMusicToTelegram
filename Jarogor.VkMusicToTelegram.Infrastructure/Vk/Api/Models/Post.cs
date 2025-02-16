using System.Collections.ObjectModel;
using Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models.JsonConverters;
using Newtonsoft.Json;
using VkNet.Model;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models;

[Serializable]
public class Post : VkNet.Model.Post {
    /// <summary>
    ///     Информация о вложениях записи (фотографии ссылки и т.п.).
    /// </summary>
    [JsonProperty("attachments")]
    [JsonConverter(typeof(CustomAttachmentJsonConverter))]
    public new ReadOnlyCollection<Attachment>? Attachments { get; set; }

    /// <summary>
    ///     Первое вложение.
    /// </summary>
    public new Attachment? Attachment => Attachments?.FirstOrDefault();
}
