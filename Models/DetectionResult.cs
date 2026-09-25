using System.ComponentModel.DataAnnotations;

namespace VERITAS.Models
{
    public class DetectionResult
    {
        public int Id { get; set; }


        // =====================================================
        // ORIGINAL NEWS HEADLINE
        // =====================================================

        [StringLength(500)]
        public string? Headline { get; set; }


        // =====================================================
        // CLAIM
        // =====================================================

        [Required]
        [StringLength(5000)]
        public string Claim { get; set; } = string.Empty;


        // =====================================================
        // SOURCE URL
        // =====================================================

        [Url]
        [StringLength(2000)]
        public string? SourceUrl { get; set; }


        // =====================================================
        // VERDICT
        // =====================================================

        [Required]
        [StringLength(50)]
        public string Result { get; set; } = string.Empty;


        // =====================================================
        // EXPLANATION
        // =====================================================

        [StringLength(5000)]
        public string? Reason { get; set; }


        // =====================================================
        // CONFIDENCE
        // =====================================================

        [Range(0, 100)]
        public double Confidence { get; set; }


        // =====================================================
        // EVIDENCE
        // =====================================================

        [StringLength(5000)]
        public string? Evidence { get; set; }


        // =====================================================
        // EVIDENCE SOURCE
        // =====================================================

        [StringLength(2000)]
        public string? Source { get; set; }


        // =====================================================
        // CHECKED DATE
        // =====================================================

        public DateTime CheckedDate { get; set; }
            = DateTime.UtcNow;
    }
}