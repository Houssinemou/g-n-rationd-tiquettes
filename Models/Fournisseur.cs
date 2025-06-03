namespace générationdétiquettes.Models
{
    public class Fournisseur
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Libelle { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty; // "Physique" ou "Moral"

        public string ICE { get; set; } = string.Empty;
    }
}
