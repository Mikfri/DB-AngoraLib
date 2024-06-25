using DB_AngoraLib.MockData;
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
        public DB_AngoraContext(DbContextOptions<DB_AngoraContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region IdentityUser PK setup - syntaks
            ////------------------- PK SETUP -------------------
            //// Irellevant, da IdentityUser allerede selv sætter disse op
            //// Configure primary key for IdentityUser
            //modelBuilder.Entity<IdentityUserClaim<string>>().HasKey(p => p.Id);
            //modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(p => new { p.LoginProvider, p.ProviderKey });
            //modelBuilder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            //modelBuilder.Entity<IdentityUserToken<string>>().HasKey(p => new { p.UserId, p.LoginProvider, p.Name });
            //modelBuilder.Entity<IdentityRoleClaim<string>>().HasKey(p => p.Id);
            #endregion

            // Configure primary key for User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            // Add unique constraint for User's BreederRegNo
            modelBuilder.Entity<User>()
                .HasIndex(u => u.BreederRegNo)
                .IsUnique();

            // Konfigurer primær nøgle for Rabbit
            modelBuilder.Entity<Rabbit>()
                    .HasKey(r => r.EarCombId);


            //------------------- FK SETUP -------------------



            // Configure Foreign Key for Rabbit -> UserOwer (ejerforhold)
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.UserOwner)        // En Rabbit har en User (UserOwner)
                .WithMany(u => u.RabbitsOwned)   // En User har mange Rabbits
                .HasForeignKey(r => r.OwnerId) // En Rabbit har en OwnerId
                .IsRequired(false);

            // Configure Foreign Key for Rabbit -> UserOrigin (Opdrætterforhold)
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.UserOrigin)
                .WithMany(u => u.RabbitsLinked)
                .HasForeignKey(r => r.OriginId)
                .IsRequired(false);


            // Configure Foreign Key for Rabbit -> Rabbit (Father)
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.Father)
                .WithMany(r => r.FatheredChildren)
                .HasForeignKey(r => r.Father_EarCombId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure Foreign Key for Rabbit -> Rabbit (Mother)
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.Mother)
                .WithMany(r => r.MotheredChildren)
                .HasForeignKey(r => r.Mother_EarCombId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete


            modelBuilder.Entity<BreederApplication>()
                .HasOne(ba => ba.UserApplicant)         // BreederApplication har én UserApplicant
                .WithMany(u => u.BreederApplications)   // User har mange BreederApplications
                .HasForeignKey(ba => ba.UserId);        // ForeignKey i BreederApplication der peger på User


            //modelBuilder.Entity<Rating>()
            //    .HasOne(r => r.Rabbit)      // En Rating har en Rabbit
            //    .WithMany(rb => rb.Ratings) // En Rabbit har mange Ratings
            //    .HasForeignKey(r => r.Id); // Opdateret til at bruge den nye Id property

        }
        public DbSet<BreederApplication> BreederApplications { get; set; }
        public DbSet<Rabbit> Rabbits { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}
