using Microsoft.EntityFrameworkCore;

namespace IcardProject.Models
{
    public class DatabaseConnection : DbContext
    {
        public DatabaseConnection(DbContextOptions<DatabaseConnection> options) : base(options)
        {
        }
        
        public DbSet<Student> Student { get; set; }
    }
}
