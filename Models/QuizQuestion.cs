using System.ComponentModel.DataAnnotations;

namespace VERITAS.Models;

public class QuizQuestion
{
    public int Id { get; set; }

    [Required]
    [StringLength(2000)]
    public string Question { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Options { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string CorrectAnswer { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Explanation { get; set; }

    [Required]
    [StringLength(50)]
    public string Difficulty { get; set; } = "Medium";
}