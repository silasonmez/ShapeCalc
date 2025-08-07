using System.Data.Entity;
using ComputeApi.Models;

namespace ComputeApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("AppDb") { } // Web.config’teki connectionStrings adı
        public DbSet<ShapeInput> ShapeInputs { get; set; }
    }
}
