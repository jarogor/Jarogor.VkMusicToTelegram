using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

[Table("public")]
public sealed class Public {
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [MaxLength(100)]
    [Column("domain")]
    public required string Domain { get; set; }

    [MaxLength(200)]
    [Column("title")]
    public required string Title { get; set; }

    public required ICollection<Post>? Posts { get; set; }
}
