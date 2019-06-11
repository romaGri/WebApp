using Library.Torrents;
using Microsoft.EntityFrameworkCore;

namespace Library
{
    public class FEDbContext : DbContext
    {
        public DbSet<Torrent> Torrents {get;set;}
        public DbSet<File> Files { get; set; }
        public DbSet<Forum> Forums { get; set; }

        public FEDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = torrentsdbTest; Trusted_Connection = True;");
        }

    }
}
