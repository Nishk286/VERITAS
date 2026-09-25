using System.ComponentModel.DataAnnotations;

namespace VERITAS.Models;

public class NewsArticle
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    [StringLength(5000)]
    public string? Content { get; set; }

    [StringLength(250)]
    public string? Source { get; set; }

    [Url]
    [StringLength(2000)]
    public string? Url { get; set; }

    public DateTime? PublishedDate { get; set; }
}