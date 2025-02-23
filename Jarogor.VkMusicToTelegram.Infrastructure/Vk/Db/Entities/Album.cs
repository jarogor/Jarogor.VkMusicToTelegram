using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jarogor.VkMusicToTelegram.Infrastructure.Vk.Db.Entities;

[Table("album")]
public sealed class Album {
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    [Required(AllowEmptyStrings = false)]
    [MaxLength(200)]
    public required string Title { get; set; }

    [Column("year")]
    [MinLength(1900), MaxLength(2050)]
    public ushort Year { get; set; }

    public ICollection<Artist> Artists { get; set; } = new List<Artist>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
