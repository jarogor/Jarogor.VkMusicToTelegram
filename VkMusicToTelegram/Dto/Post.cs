﻿using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VkNet.Model;

namespace VkMusicToTelegram.Dto;

[Serializable]
public class Post : VkNet.Model.Post
{
    /// <summary>
    /// Информация о вложениях записи (фотографии ссылки и т.п.).
    /// </summary>
    [JsonProperty("attachments")]
    [JsonConverter(typeof(CustomAttachmentJsonConverter))]
    public new ReadOnlyCollection<Attachment> Attachments { get; set; }

    /// <summary>
    /// Первое вложение.
    /// </summary>
    public new Attachment Attachment => Attachments.FirstOrDefault();
}