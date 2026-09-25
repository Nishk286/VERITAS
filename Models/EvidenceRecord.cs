using System.ComponentModel.DataAnnotations;

namespace VERITAS.Models
{
    public class EvidenceRecord
    {
        public int Id { get; set; }

        [Required]
        [StringLength(5000)]
        public string Claim { get; set; } = "";

        [StringLength(5000)]
        public string Evidence { get; set; } = "";

        [StringLength(2000)]
        public string Source { get; set; } = "";

        [StringLength(50)]
        public string EvidenceType { get; set; } = "Unknown";

        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
    }
}