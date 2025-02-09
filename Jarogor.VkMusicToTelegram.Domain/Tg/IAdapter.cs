namespace Jarogor.VkMusicToTelegram.Domain.Tg;

public interface IAdapter {
    public Task SendMessage(string message, CancellationToken stoppingToken);
    public Task SendMessage(Dictionary<string, List<Result>> items, CancellationToken stoppingToken);
}
