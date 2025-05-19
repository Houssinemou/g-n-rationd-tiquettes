namespace générationdétiquettes.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string? CodeArticle { get; set; }
        public string Nom { get; set; } = null!;
        public string? Famille { get; set; }
        public string? Localisation { get; set; }
        public string? Fournisseur { get; set; }
        public string? UniteFonction { get; set; }
        public string? Statut { get; set; }
        public string? MarqueModele { get; set; }
        public string? NumeroSerie { get; set; }
        public DateTime? DateAcquisition { get; set; }
        public DateTime? DateEnregistrement { get; set; }
        public decimal? PrixAcquisition { get; set; }
        public string? ModeleEtiquette { get; set; }
        public string? PhotoPath { get; set; }
        public string? PieceJointePath { get; set; }

        public int? BarcodeId { get; set; } // Nullable si l'article peut ne pas avoir de code-barre

    }
}
