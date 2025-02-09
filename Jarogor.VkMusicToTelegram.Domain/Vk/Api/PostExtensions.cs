namespace Jarogor.VkMusicToTelegram.Domain.Vk.Api;

public static class PostExtensions {
    public static bool IsSkip(this Post p)
        => p.IsPinned.HasValue && p.IsPinned.GetValueOrDefault() // Кроме закреплённых постов
           || !p.OwnerId.HasValue
           || !p.Id.HasValue
           || p.Attachments.Count == 0;
}
