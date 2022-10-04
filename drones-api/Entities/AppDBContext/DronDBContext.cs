using Microsoft.EntityFrameworkCore;

namespace drones_api.Entities.AppDBContext
{
    public class DronDBContext: DbContext
    {
        public DronDBContext(DbContextOptions<DronDBContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Dron>().HasKey(e => e.SerialNumber);
            modelBuilder.Entity<Dron>().Property(e=>e.SerialNumber)
                .HasMaxLength(100)
                .IsRequired()
                .ValueGeneratedNever();
            modelBuilder.Entity<Dron>().Property(e => e.BateryLevel).HasMaxLength(3);
            modelBuilder.Entity<Dron>().Property(e => e.Model).HasMaxLength(50);
            modelBuilder.Entity<Dron>().Property(e => e.State).HasMaxLength(30);
            modelBuilder.Entity<Dron>().Property(e => e.State).HasConversion<string>().HasMaxLength(30);
            modelBuilder.Entity<Dron>().HasMany<Medicine>(e => e.Medicines);


            modelBuilder.Entity<Medicine>().HasKey(e => e.Code);
        }

        //entities
        public DbSet<Dron> Drones { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
    }
}
