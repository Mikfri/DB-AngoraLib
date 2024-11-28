using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.SeededData;
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

            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<Breeder>("Breeder");

            // Configure primary key for User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            // Add unique constraint for Breeder's BreederRegNo
            modelBuilder.Entity<Breeder>()
                .HasIndex(b => b.BreederRegNo)
                .IsUnique();

            // Konfigurer primær nøgle for Rabbit
            modelBuilder.Entity<Rabbit>()
                .HasKey(r => r.EarCombId);

            modelBuilder.Entity<TransferRequst>()
                .HasIndex(rt => rt.RabbitId);


            //------------------- FK SETUP -------------------

            modelBuilder.Entity<BreederBrand>()
                .HasOne(bb => bb.BreederBrandOwner)
                .WithOne(u => u.BreederBrand)
                .HasForeignKey<BreederBrand>(bb => bb.UserId)
                //.HasPrincipalKey<User>(u => u.BreederRegNo) // Angiv at User's BreederRegNo er hovednøglen
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for at slette BreederBrand, når en User slettes

            // Configure Foreign Key for Rabbit -> UserOwer (Ejerforhold)
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.UserOwner)        // En Rabbit har en User (UserOwner)
                .WithMany(u => u.RabbitsOwned)   // En User har mange Rabbits
                .HasForeignKey(r => r.OwnerId)   // En Rabbit har en OwnerId
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
                .OnDelete(DeleteBehavior.NoAction); // No action on delete

            // Configure Foreign Key for Rabbit -> Rabbit (Mother)
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.Mother)
                .WithMany(r => r.MotheredChildren)
                .HasForeignKey(r => r.Mother_EarCombId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction); // No action on delete

            modelBuilder.Entity<Trimming>()
                .HasOne(t => t.Rabbit)
                .WithMany(r => r.Trimmings)
                .HasForeignKey(t => t.RabbitId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for at slette trimmings, når en rabbit slettes


            modelBuilder.Entity<ApplicationBreeder>()
                .HasOne(ba => ba.UserApplicant)           // ApplicationBreeder har én UserApplicant
                .WithMany(u => u.BreederApplications)     // User har mange ApplicationBreeder
                .HasForeignKey(ba => ba.UserApplicantId); // ForeignKey i ApplicationBreeder der peger på User


            //modelBuilder.Entity<Rating>()
            //    .HasOne(r => r.Rabbit)      // En Rating har en Rabbit
            //    .WithMany(rb => rb.Ratings) // En Rabbit har mange Ratings
            //    .HasForeignKey(r => r.Id); // Opdateret til at bruge den nye Id property


            // Konfigurer RabbitTransferRequest relationer
            modelBuilder.Entity<TransferRequst>()
                .HasOne(rt => rt.Rabbit)
                .WithMany()
                .HasForeignKey(rt => rt.RabbitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferRequst>()
                .HasOne(rt => rt.UserIssuer)
                .WithMany(rt => rt.RabbitTransfers_Issued)
                .HasForeignKey(rt => rt.IssuerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferRequst>()
                .HasOne(rt => rt.UserRecipent)
                .WithMany(rt => rt.RabbitTransfers_Received)
                .HasForeignKey(rt => rt.RecipentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Konfigurer RefreshToken relationen
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User) // Antager at RefreshToken har en navigationsegenskab 'User'
                .WithMany(u => u.RefreshTokens) // Bruger 'RefreshTokens' som den inverse navigationsegenskab
                .HasForeignKey(rt => rt.UserId); // Antager at RefreshToken har en 'UserId' foreign key

            modelBuilder.Seed();
        }

        public DbSet<ApplicationBreeder> BreederApplications { get; set; }
        public DbSet<BreederBrand> BreederBrands { get; set; }
        public DbSet<Rabbit> Rabbits { get; set; }
        public DbSet<Trimming> Trimmings { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TransferRequst> TransferRequests { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}