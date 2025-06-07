using System.ComponentModel.DataAnnotations;

namespace g�n�rationd�tiquettes.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public byte[] PasswordSalt { get; set; } = [];

        public string Role { get; set; } = "Administrateur";
    }
}
