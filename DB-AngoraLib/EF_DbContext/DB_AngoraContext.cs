using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.EF_DbContext
{
    public class DB_AngoraContext : IdentityDbContext<User>
    {

        #region //---------------- UDEN: SECRET SETUP ----------------
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
        #region //---------------- MED: SECRET SETUP ----------------
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
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //------------------- PK SETUP -------------------
            // Configure primary key for IdentityUser
            modelBuilder.Entity<IdentityUserClaim<string>>().HasKey(p => p.Id);
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(p => new { p.LoginProvider, p.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(p => new { p.UserId, p.LoginProvider, p.Name });
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasKey(p => p.Id);

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

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Rabbit) // En Rating har en Rabbit
                .WithMany(rb => rb.Ratings) // En Rabbit har mange Ratings
                .HasForeignKey(r => new { r.RightEarId, r.LeftEarId });

            modelBuilder.Entity<RabbitParents>()
                .HasOne(rp => rp.RabbitMother)      // En RabbitParent har en RabbitMother
                .WithMany(r => r.MotheredChildren)
                .HasForeignKey(rp => new { rp.MotherRightEarId, rp.MotherLeftEarId })
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RabbitParents>()
                .HasOne(rp => rp.RabbitFather)
                .WithMany(r => r.FatheredChildren)
                .HasForeignKey(rp => new { rp.FatherRightEarId, rp.FatherLeftEarId })
                .OnDelete(DeleteBehavior.NoAction);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Rabbit> Rabbits { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RabbitParents> RabbitParents { get; set; }
        public DbSet<Photo> Photos { get; set; }

    }
}
