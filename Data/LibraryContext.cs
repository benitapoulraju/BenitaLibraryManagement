using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;
namespace LibraryManagement.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }

        
        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options) 
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\Local;Database=LibraryDB;Trusted_Connection=True;MultipleActiveResultSets=true;");
        }
    }
}





