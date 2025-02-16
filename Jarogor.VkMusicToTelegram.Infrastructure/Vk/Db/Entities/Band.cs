using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

[Table("band")]
public sealed class Band {
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [MaxLength(200)]
    public required string Name { get; set; }

    public ICollection<Album>? Albums { get; set; }
}
