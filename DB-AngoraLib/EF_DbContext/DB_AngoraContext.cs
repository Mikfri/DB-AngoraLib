using DB_AngoraLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.EF_DbContext
{
    public class DB_AngoraContext : DbContext
    {

        #region //---------------- BASIC SETUP FØR SECRET SETUP ----------------
        public DB_AngoraContext(DbContextOptions<DB_AngoraContext> options) : base(options) { }

        public DB_AngoraContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                      @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DB-Angora_DB; Integrated Security=True; Connect Timeout=30; Encrypt=False");
            }
        }
        #endregion
        //private readonly IConfiguration _configuration;

        //public DB_AngoraContext(DbContextOptions<DB_AngoraContext> options, IConfiguration configuration) : base(options)
        //{
        //    _configuration = configuration;
        //}
        //public DB_AngoraContext() { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SECRETConnection"));
        //    }
        //}

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
                .HasOne(r => r.User)       // En Rabbit har en User
                .WithMany(u => u.Rabbits)   // En User har mange Rabbits
                .HasForeignKey(r => r.OwnerId); // En Rabbit har en OwnerId

            // Configure Foreign Key for Rating -> Rabbit
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Rabbit) // En Rating har en Rabbit
                .WithMany(rb => rb.Ratings) // En Rabbit har mange Ratings
                .HasForeignKey(r => new { r.RightEarId, r.LeftEarId });

            // Configure composite key for RabbitParents
            // Configure Foreign Key for RabbitParents -> Mother
            modelBuilder.Entity<RabbitParents>()
                .HasOne(rp => rp.RabbitMother)
                .WithMany(r => r.MotheredChildren)
                .HasForeignKey(rp => new { rp.MotherRightEarId, rp.MotherLeftEarId })
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Foreign Key for RabbitParents -> Father
            modelBuilder.Entity<RabbitParents>()
                .HasOne(rp => rp.RabbitFather)
                .WithMany(r => r.FatheredChildren)
                .HasForeignKey(rp => new { rp.FatherRightEarId, rp.FatherLeftEarId })
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<Rabbit> Rabbits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RabbitParents> RabbitParents { get; set; }
        public DbSet<Photo> Photos { get; set; }

    }
}
