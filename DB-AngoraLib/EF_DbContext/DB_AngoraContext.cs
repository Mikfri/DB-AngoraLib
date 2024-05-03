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
            //------------------- PK SETUP -------------------
            // Configure primary key for IdentityUser
            modelBuilder.Entity<IdentityUserClaim<string>>().HasKey(p => p.Id);
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(p => new { p.LoginProvider, p.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(p => new { p.UserId, p.LoginProvider, p.Name });
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasKey(p => p.Id);

            // Configure primary key for User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
                        
            // Configure composite key for Rabbit
            modelBuilder.Entity<Rabbit>()
                .HasKey(r => new { r.RightEarId, r.LeftEarId });

            //------------------- FK SETUP -------------------
            // Configure Foreign Key for Rabbit -> User
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.User)       // En Rabbit har en User
                .WithMany(u => u.Rabbits)   // En User har mange Rabbits
                .HasForeignKey(r => r.OwnerId) // En Rabbit har en OwnerId
                .IsRequired(false);

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


            // Tilføj mock data
            //var passwordHasher = new PasswordHasher<User>();    // Tilføj passwordHasher
            //var mockUsers = new MockUsers(passwordHasher);      // Tilføj mockUsers med passwordHasher
            //var users = mockUsers.GetMockUsers();               // users er en liste af mockUsers
            //modelBuilder.Entity<User>().HasData(users);         // Tilføj users til User tabel

            //var mockRabbits = MockRabbits.GetMockRabbits();
            //modelBuilder.Entity<Rabbit>().HasData(mockRabbits);

            //var mockUsers = MockUsers.GetMockUsers();           
            //modelBuilder.Entity<User>().HasData(mockUsers);       //todo: hvis vi fjerne den automatiske gen af MockUser og Rabbits og istedet objekterne MST via Mock virker ICollections

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Rabbit> Rabbits { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RabbitParents> RabbitParents { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}
