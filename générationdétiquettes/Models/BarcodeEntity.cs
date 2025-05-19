using System.ComponentModel.DataAnnotations;

namespace générationdétiquettes.Models
{
    public class BarcodeEntity
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Base64Image { get; set; }
        public string LogoPath { get; set; } // Chemin du logo dans le serveur (optionnel)
        public string CodeFamille { get; set; }
        public string LibelleFamille { get; set; }
        public string CodeLocalisation { get; set; }
        public string LibelleLocalisation { get; set; }
        public string Texte { get; set; }

        public DateTime CreatedAt { get; set; } // Ensure this property is included only once
    }
}
