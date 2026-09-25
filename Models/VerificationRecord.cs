using System.ComponentModel.DataAnnotations;

namespace VERITAS.Models;

public class VerificationRecord
{
    public int Id { get; set; }

    [Required]
    [StringLength(5000)]
    public string Claim { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Result { get; set; } = string.Empty;

    [StringLength(5000)]
    public string? Evidence { get; set; }

    [StringLength(2000)]
    public string? Source { get; set; }

    public DateTime CheckedDate { get; set; } = DateTime.UtcNow;
}