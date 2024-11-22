using Jarogor.VkMusicToTelegram.Dto;

namespace Jarogor.VkMusicToTelegram.PostHandlers.Handlers;

/// <summary>
/// - первая строка это название `группа - альбом (год)`
/// - название группы может выглядеть так `[club787866|MINUALA]`, поэтому его нужно проверять регуляркой и вырезать
/// - последняя строка это теги, но имеющие разделитель `|` отделяющий теги музыки от каких-то служебных, можно пробовать отрезать
/// </summary>
public sealed class BlackWall : Handler1Base {
    public override string Domain => "blackwall";
    public override string Name => "Blackwall";
    public override Record GetPreparedTitle(Post post) => base.PrepareTitle(post);
}
