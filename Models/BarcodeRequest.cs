using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace GenerationEtiquettes.Models
{
    public class BarcodeRequest
    {
        [BindNever]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [Display(AutoGenerateField = false)]
        public string Code { get; set; } = "";


        [Required(ErrorMessage = "Le préfixe est obligatoire.")]
        public string Prefix { get; set; }

        [Required(ErrorMessage = "Le type est obligatoire.")]
        [RegularExpression("qr|1d", ErrorMessage = "Le type doit être 'qr' ou '1d'.")]
        public string Type { get; set; }

        public string Description { get; set; }
        public string CodeFamille { get; set; }
        public string LibelleFamille { get; set; }
        public string CodeLocalisation { get; set; }
        public string LibelleLocalisation { get; set; }
        public string Texte { get; set; }
        public string LogoBase64 { get; set; }

        public DateTime? DateAcquisition { get; set; }
        public DateTime? DateEnregistrement { get; set; }

        [JsonPropertyName("contenuQRCode")]
        public string? ContenuQRCode { get; set; }

        [Required(ErrorMessage = "Le layout est obligatoire.")]
        public List<LayoutElement> Layout { get; set; } = new();
    }

    public class LayoutElement
    {
        public string Id { get; set; }
        public bool Visible { get; set; }
        public float Top { get; set; }
        public float Left { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string FontSize { get; set; }
        public string FontWeight { get; set; }
        public string FontStyle { get; set; }
        public string Color { get; set; }
        public string TextAlign { get; set; }
    }
}
