using Newtonsoft.Json;

namespace Jarogor.VkMusicToTelegram.Dto;

[Serializable]
public class Link : VkNet.Model.Link {
    [JsonProperty("ref", NullValueHandling = NullValueHandling.Ignore)]
    public string? Ref { get; set; }

    public bool IsAudioPlaylist => Ref is not null && Ref == "audio_playlist";
}
