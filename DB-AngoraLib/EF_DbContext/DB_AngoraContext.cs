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
        public DB_AngoraContext() : this(new DbContextOptionsBuilder<DB_AngoraContext>()
            .UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DB-Angora_DB; Integrated Security=True; Connect Timeout=30; Encrypt=False")
            .Options)
        {
        }

        public DB_AngoraContext(DbContextOptions<DB_AngoraContext> options) : base(options)  {   }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //------------------- PK SETUP -------------------
            // Configure primary key for User
            modelBuilder.Entity<User>()
                .HasKey(u => u.BreederRegNo);

            // Configure composite key for Rabbit
            modelBuilder.Entity<Rabbit>()
                .HasKey(r => new { r.RightEarId, r.LeftEarId });

            //------------------- FK SETUP -------------------
            // Configure Foreign Key for Rabbit -> User
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.Owner)
                .WithMany(u => u.Rabbits)
                .HasForeignKey(r => r.OwnerId);

            // Configure composite key for RabbitParents
            // Configure Foreign Key for RabbitParents -> Mother
            modelBuilder.Entity<RabbitParents>()
                .HasOne(rp => rp.Mother)
                .WithMany(r => r.MotherChildren)
                .HasForeignKey(rp => new { rp.MotherRightEarId, rp.MotherLeftEarId })
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Foreign Key for RabbitParents -> Father
            modelBuilder.Entity<RabbitParents>()
                .HasOne(rp => rp.Father)
                .WithMany(r => r.FatherChildren)
                .HasForeignKey(rp => new { rp.FatherRightEarId, rp.FatherLeftEarId })
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<Rabbit> Rabbits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RabbitParents> RabbitParents { get; set; }
    }
}
