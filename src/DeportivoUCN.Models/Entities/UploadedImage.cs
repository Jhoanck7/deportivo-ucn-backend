using System.ComponentModel.DataAnnotations;

namespace DeportivoUCN.Models.Entities;

public class UploadedImage
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    public byte[] Data { get; set; } = [];

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
