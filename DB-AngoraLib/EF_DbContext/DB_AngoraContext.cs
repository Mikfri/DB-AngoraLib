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
            options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DB-Angora_Db; Integrated Security=True; Connect Timeout=30; Encrypt=False");
        }

        public DbSet<Rabbit> Rabbits { get; set; }

    }
}
