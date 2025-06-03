// Models/Localisation.cs
using System.ComponentModel.DataAnnotations;

namespace générationdétiquettes.Models
{
    public class Localisation
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Libelle { get; set; } = string.Empty;

        public string? Etage { get; set; }   // Ex: Etage 2
        public string? Batiment { get; set; } // Ex: Imm Blanc
        public string? Details { get; set; } // Ex: Local Involys
    }
}
