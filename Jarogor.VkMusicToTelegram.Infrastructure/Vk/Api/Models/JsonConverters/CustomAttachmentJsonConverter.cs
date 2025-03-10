﻿using System.Collections;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNet.Model;
using VkNet.Utils;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Api.Models.JsonConverters;

public class CustomAttachmentJsonConverter : JsonConverter {
    /// <inheritdoc />
    public override bool CanConvert(Type objectType) => typeof(ReadOnlyCollection<>).IsAssignableFrom(objectType);

    /// <inheritdoc />
    /// <exception cref="T:System.NotImplementedException"> </exception>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        var attachments = (IEnumerable<Attachment>)value!;

        var jArray = new JArray();

        foreach (var attachment in attachments) {
            var type = attachment.Type.Name.ToLower();

            var jObj = new JObject {
                { "type", type },
                { type, JToken.FromObject(attachment.Instance, serializer) },
            };

            jArray.Add(jObj);
        }

        jArray.WriteTo(writer);
    }

    /// <inheritdoc />
    /// <exception cref="T:System.TypeAccessException"> </exception>
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
        if (objectType is null || !objectType.IsGenericType) {
            throw new TypeAccessException();
        }

        if (reader.TokenType is JsonToken.Null) {
            return null;
        }

        if (reader.TokenType is not JsonToken.StartArray) {
            return null;
        }

        var keyType = objectType.GetGenericArguments()[0];
        var constructedListType = typeof(List<>).MakeGenericType(keyType);
        if (Activator.CreateInstance(constructedListType) is not IList list) {
            return null;
        }

        var obj = JArray.Load(reader);
        foreach (var item in obj) {
            list.Add(AttachmentConverterService.Instance.CustomLinkFromJson(item));
        }

        var vkCollection = typeof(ReadOnlyCollection<>).MakeGenericType(keyType);
        return Activator.CreateInstance(vkCollection, list);
    }
}
