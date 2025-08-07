using System.Data.Entity;
using InputApi.Models;

namespace InputApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("AppDb") { } // Web.config’teki connectionStrings adı
        public DbSet<ShapeInput> ShapeInputs { get; set; }
    }
}
