using day_1.Models;
using Microsoft.EntityFrameworkCore;

namespace day_1.Data
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<student> students { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
