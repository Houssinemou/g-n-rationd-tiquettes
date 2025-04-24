using System.ComponentModel.DataAnnotations;

namespace GenerationEtiquettes.Models
{
    public class BarcodeRequest
    {
        [Required(ErrorMessage = "Le code est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le code ne peut excéder 50 caractères.")]
        public string Code { get; set; }

        [StringLength(100, ErrorMessage = "La description ne peut excéder 100 caractères.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le type est obligatoire.")]
        [RegularExpression("qr|1d", ErrorMessage = "Le type doit être 'qr' ou '1d'.")]
        public string Type { get; set; }

        [StringLength(20, ErrorMessage = "Le code famille ne peut excéder 20 caractères.")]
        public string CodeFamille { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Le libellé famille ne peut excéder 50 caractères.")]
        public string LibelleFamille { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Le code localisation ne peut excéder 20 caractères.")]
        public string CodeLocalisation { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Le libellé localisation ne peut excéder 50 caractères.")]
        public string LibelleLocalisation { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Le texte ne peut excéder 200 caractères.")]
        public string Texte { get; set; } = string.Empty;
    }
}