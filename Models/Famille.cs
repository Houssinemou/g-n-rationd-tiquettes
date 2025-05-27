using System.ComponentModel.DataAnnotations;

namespace générationdétiquettes.Models
{
    public class Famille
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Libelle { get; set; } = string.Empty;
    }
}
