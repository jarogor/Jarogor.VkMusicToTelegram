namespace VkMusicToTelegram;

public sealed class TopCount(int month, int week) {
    public int Month { get; private set; } = month;
    public int Week { get; private set; } = week;
}
