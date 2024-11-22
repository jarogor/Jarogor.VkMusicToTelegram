using Jarogor.VkMusicToTelegram.Dto;

namespace Jarogor.VkMusicToTelegram.PostHandlers.Handlers;

/// <summary>
///     - первая строка это теги,
///     - вторая непустая строка это название вида `#Какая_То_Группа - название альбома и всё такое`, поэтому оно не
///     годится
///     - название можно брать из плейлиста, у них оно более-менее единообразно оформленное с правильным написанием `группа
///     - альбом (год)`
/// </summary>
public sealed class AsylumForesters : IHandler {
    public string Domain => "asylumforesters_vk";
    public string Name => "Убежище Лесников";

    public Record GetPreparedTitle(Post post) {
        var playlistsNames = post
            .Attachments
            .Where(it => it.Type == typeof(Link))
            .Select(it => it.Instance)
            .Cast<Link>()
            .Where(it => it.IsAudioPlaylist)
            .Select(it => it.Title)
            .ToList();

        return playlistsNames.Count <= 0
            ? new NullRecord()
            : new Record(string.Join(",", playlistsNames), true);
    }
}
