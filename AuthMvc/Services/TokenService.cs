using AuthMvc.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(UserModel user)
    {
        // Ambil nilai key dari konfigurasi
        var key = _config["Jwt:Key"];

        // Validasi apakah key tidak kosong atau null
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException("Jwt:Key is not found or is empty.");
        }

        // Membuat SymmetricSecurityKey dari key yang sudah diambil
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        // Membuat kredensial tanda tangan
        var creds = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

        // Membuat token JWT
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        // Mengembalikan token yang dihasilkan dalam format string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
