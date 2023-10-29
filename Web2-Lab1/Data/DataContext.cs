using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SampleMvcApp.Data.Entities;

namespace SampleMvcApp.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Natjecanje> Natjecanja { get; set; }
        public DbSet<Natjecatelj> Natjecatelji { get; set; }
        public DbSet<Rezultat> Rezultati { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
