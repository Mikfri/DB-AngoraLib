using DB_AngoraLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.EF_DbContext
{
    public class DB_AngoraContext : DbContext
    {
        public DB_AngoraContext(DbContextOptions<DB_AngoraContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DB-Angora_DB; Integrated Security=True; Connect Timeout=30; Encrypt=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurer sammensat nøgle for Rabbit
            modelBuilder.Entity<Rabbit>()
                .HasKey(r => r.Id);

            // Konfigurer Foreign Key for Rabbit -> User
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.Owner) // eller hvad du nu ønsker at navngive det
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasKey(u => u.BreederRegNo); // Brug BreederRegNo som primærnøgle
        }


        public DbSet<Rabbit> Rabbits { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
