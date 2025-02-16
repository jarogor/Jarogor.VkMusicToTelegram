using System.Collections.ObjectModel;

namespace Jarogor.VkMusicToTelegram.Domain.Vk.Api;

public sealed class Post {
    /// <summary>
    /// Текст записи.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Время публикации записи.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Если запись закрепленная - вернет <c>true</c>
    /// </summary>
    public bool? IsPinned { get; set; }

    /// <summary>
    /// Идентификатор вложенеия.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Идентификатор владельца вложения.
    /// </summary>
    public long OwnerId { get; set; }

    /// <summary>
    /// Информация о вложениях записи (фотографии ссылки и т.п.).
    /// </summary>
    public ReadOnlyCollection<string> Attachments { get; set; } = ReadOnlyCollection<string>.Empty;

    /// <summary>
    /// Информация о лайках к записи.
    /// </summary>
    public int Likes { get; set; }

    /// <summary>
    /// Информация о просмотрах записи.
    /// </summary>
    public int Views { get; set; }

    /// <summary>
    /// Информация о репостах записи.
    /// </summary>
    public int Reposts { get; set; }
}
