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
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DB-Angora_DB; Integrated Security=True; Connect Timeout=30; Encrypt=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurer sammensat nøgle for Rabbit
            modelBuilder.Entity<Rabbit>()
                .HasKey(r => new { r.RightEarId, r.LeftEarId });

            // Konfigurer Foreign Key for Rabbit -> User
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rabbits)
                .HasForeignKey(r => r.Owner)
                .OnDelete(DeleteBehavior.Restrict);  // Juster dette efter dine sletningskrav
        }


        public DbSet<Rabbit> Rabbits { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
