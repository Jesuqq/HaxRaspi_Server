using HaxRaspi_Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaxRaspi_Server.Helpers {
    public class DataContext : DbContext {
        public DataContext (DbContextOptions<DataContext> options) : base (options) { }

        public DbSet<Interface> Interfaces { get; set; }

    }
}