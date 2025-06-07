using générationdétiquettes.Models;
using générationdétiquettes.Data;
using générationdétiquettes.Models;
using générationdétiquettes.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


public interface IAuthService

{
    Task<string> Register(UserDto request);
    Task<string> Login(UserDto request);
    string CreateToken(User user);
}
public class AuthService : IAuthService
{
    private readonly BarcodeDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(BarcodeDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<string> Register(UserDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            throw new Exception("Utilisateur déjà existant");

        CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = Convert.ToBase64String(hash),
            PasswordSalt = salt,
            Role = "Administrateur"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreateToken(user);
    }

    public async Task<string> Login(UserDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new Exception("Identifiants invalides");

        return CreateToken(user);
    }

    private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, string storedHash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(Convert.FromBase64String(storedHash));
    }


    public string CreateToken(User user)
    {
        var claims = new[]
        {
                new Claim("username", user.Username),
                new Claim("role", user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
