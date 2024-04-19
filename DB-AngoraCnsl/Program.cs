using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using Microsoft.EntityFrameworkCore;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        using (var context = new DB_AngoraContext())
        {
            // Sørg for, at databasen er oprettet.
            context.Database.EnsureCreated();

            // Kig efter kaniner.
            if (!context.Rabbits.Any())
            {
                var users = MockUsers.GetMockUsers().ToArray();
                var rabbits = MockRabbits.GetMockRabbits().ToArray();

                // Tilføj mock brugere til databasen
                context.Users.AddRange(users);

                // Tilføj mock kaniner til databasen
                context.Rabbits.AddRange(rabbits);

                // Gem ændringerne i databasen
                context.SaveChanges();
            }
        }
    }
}