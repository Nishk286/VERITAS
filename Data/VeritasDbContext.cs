using Microsoft.EntityFrameworkCore;
using VERITAS.Models;

namespace VERITAS.Data
{
    public class VeritasDbContext : DbContext
    {
        public VeritasDbContext(
            DbContextOptions<VeritasDbContext> options)
            : base(options)
        {
        }

        // News articles stored in VERITAS
        public DbSet<NewsArticle> NewsArticles { get; set; }

        // Detection results
        public DbSet<DetectionResult> DetectionResults { get; set; }

        // Verification records
        public DbSet<VerificationRecord> VerificationRecords { get; set; }

        // Quiz questions
        public DbSet<QuizQuestion> QuizQuestions { get; set; }

        // Evidence records
        public DbSet<EvidenceRecord> EvidenceRecords { get; set; }
    }
}