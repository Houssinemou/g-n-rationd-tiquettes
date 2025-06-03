using Microsoft.EntityFrameworkCore;
using générationdétiquettes.Models;


namespace générationdétiquettes.Data
{
    public class BarcodeDbContext : DbContext
    {
        public BarcodeDbContext(DbContextOptions<BarcodeDbContext> options) : base(options)
        {
        }

        // Tables
        public DbSet<BarcodeEntity> Barcodes { get; set; }
        public DbSet<CodeSequence> CodeSequences { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Famille> Familles { get; set; }
        public DbSet<UniteFonction> UnitesFonction { get; set; }
        public DbSet<Fournisseur> Fournisseurs { get; set; }
        public DbSet<Localisation> Localisations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // BarcodeEntity
            modelBuilder.Entity<BarcodeEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.Type).IsRequired();

                entity.Property(e => e.Description).IsRequired(false);
                entity.Property(e => e.CodeFamille).IsRequired(false);
                entity.Property(e => e.LibelleFamille).IsRequired(false);
                entity.Property(e => e.CodeLocalisation).IsRequired(false);
                entity.Property(e => e.LibelleLocalisation).IsRequired(false);
                entity.Property(e => e.Texte).IsRequired(false);
                entity.Property(e => e.LogoPath).IsRequired(false);
                entity.Property(e => e.Base64Image).IsRequired(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // CodeSequence
            modelBuilder.Entity<CodeSequence>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Prefix).IsRequired();
                entity.Property(e => e.LastNumber).IsRequired();
            });

            // Article
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CodeArticle).IsRequired();
                entity.Property(e => e.Nom).IsRequired();

                entity.Property(e => e.Famille).IsRequired(false);
                entity.Property(e => e.Localisation).IsRequired(false);
                entity.Property(e => e.Fournisseur).IsRequired(false);
                entity.Property(e => e.UniteFonction).IsRequired(false);
                entity.Property(e => e.Statut).IsRequired(false);
                entity.Property(e => e.MarqueModele).IsRequired(false);
                entity.Property(e => e.NumeroSerie).IsRequired(false);
                entity.Property(e => e.DateAcquisition).IsRequired(false);
                entity.Property(e => e.DateEnregistrement).IsRequired(false);
                entity.Property(e => e.PrixAcquisition).IsRequired(false);
                entity.Property(e => e.ModeleEtiquette).IsRequired(false);
                entity.Property(e => e.PhotoPath).IsRequired(false);
                entity.Property(e => e.PieceJointePath).IsRequired(false);
            });
        }
    }
}
