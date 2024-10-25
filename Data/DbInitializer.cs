using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LibraryManagement.Data
{
    public static class DbInitializer
    {
        public static void Seed(LibraryContext context)
        {
            if (context.Database.CanConnect())
            {
                ExecuteSqlScript(context, "DatabaseScript/PreScript.txt");
                
                ExecuteSqlScript(context, "DatabaseScript/PostScript.txt");
            }
            else
            {
                context.Database.EnsureCreated();
            }
        }

        private static void ExecuteSqlScript(LibraryContext context, string scriptPath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), scriptPath);
            var sql = File.ReadAllText(fullPath);
            context.Database.ExecuteSqlRaw(sql);
        }
    }
}
