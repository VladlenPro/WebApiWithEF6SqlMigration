using Microsoft.EntityFrameworkCore;
using WebApiWithEF6SqlMigration.Models;

namespace WebApiWithEF6SqlMigration.Data
{
    public class PersonApiDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public PersonApiDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
