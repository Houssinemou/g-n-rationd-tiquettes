using Microsoft.EntityFrameworkCore;
using générationdétiquettes.Models;

namespace générationdétiquettes.Data
{
    public class BarcodeDbContext : DbContext
    {
        public BarcodeDbContext(DbContextOptions<BarcodeDbContext> options) : base(options)
        {
        }

        public DbSet<BarcodeEntity> Barcodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BarcodeEntity>(entity =>
            {
                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.Type).IsRequired();

                // Tous les autres champs sont optionnels
                entity.Property(e => e.Description).IsRequired(false);
                entity.Property(e => e.CodeFamille).IsRequired(false);
                entity.Property(e => e.LibelleFamille).IsRequired(false);
                entity.Property(e => e.CodeLocalisation).IsRequired(false);
                entity.Property(e => e.LibelleLocalisation).IsRequired(false);
                entity.Property(e => e.Texte).IsRequired(false);
            });
        }
    }
}
