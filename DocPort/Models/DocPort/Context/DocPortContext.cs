using DocPort.Models.DocPort.Models;
using Microsoft.EntityFrameworkCore;

namespace DocPort.Models.DocPort.Context
{
    public class DocPortContext : DbContext
    {
        public DocPortContext(DbContextOptions<DocPortContext> options)
            : base(options) { }

        public virtual DbSet<Doc> Docs {  get; set; }
        public virtual DbSet<ContentHolder> ContentHolders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
