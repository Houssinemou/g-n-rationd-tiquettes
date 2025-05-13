using System.ComponentModel.DataAnnotations;

namespace générationdétiquettes.Models
{
    public class CodeSequence
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        public string Prefix { get; set; } = "";

        [Required]
        public int LastNumber { get; set; } = 0;
    }
}
