using System.ComponentModel.DataAnnotations;

namespace générationdétiquettes.Models
{
    public class BarcodeEntity
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }  // Add this missing property
        public string Type { get; set; }        // Add this missing property
        public string Base64Image { get; set; } // Renamed from ImageBase64 to match usage

        public string CodeFamille { get; set; }
        public string LibelleFamille { get; set; }
        public string CodeLocalisation { get; set; }
        public string LibelleLocalisation { get; set; }
        public string Texte { get; set; }
    }
}
