using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VERITAS.Data
{
    public class VeritasDbContextFactory
        : IDesignTimeDbContextFactory<VeritasDbContext>
    {
        public VeritasDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder =
                new DbContextOptionsBuilder<VeritasDbContext>();

            optionsBuilder.UseSqlite("Data Source=veritas.db");

            return new VeritasDbContext(optionsBuilder.Options);
        }
    }
}