using Microsoft.EntityFrameworkCore;
using restier_swagger_bug_report.Models;

namespace restier_swagger_bug_report
{
    public class SampleDbContext : DbContext
    {
        public virtual DbSet<Person> Persons { get; set; }


        public SampleDbContext()
        {
        }

        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        {
        }
    }
}
