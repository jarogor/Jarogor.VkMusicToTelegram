using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

[Table("post")]
public sealed class Post {
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("views")]
    public int Views { get; set; }

    [Column("likes")]
    public int Likes { get; set; }

    [Column("reposts")]
    public int Reposts { get; set; }

    public Public? Public { get; set; }
    public Album? Album { get; set; }
}
