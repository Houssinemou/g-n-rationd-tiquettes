using System.ComponentModel.DataAnnotations;

namespace g�n�rationd�tiquettes.Models
{
    public class UniteFonction
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Libelle { get; set; } = string.Empty;
    }
}