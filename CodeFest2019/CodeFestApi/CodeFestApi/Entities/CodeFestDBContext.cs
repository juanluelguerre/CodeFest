using Microsoft.EntityFrameworkCore;

namespace CodeFest.Api.Entities
{
	public partial class CodeFestDBContext : DbContext
    {
        public CodeFestDBContext()
        {
        }

        public CodeFestDBContext(DbContextOptions<CodeFestDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Twitter> Twitter { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=tcp:codefestdbserver.database.windows.net,1433;Initial Catalog=CodeFestDB;Persist Security Info=False;User ID=juanlu;Password=CodeFest2019!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Twitter>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });
        }
    }
}
