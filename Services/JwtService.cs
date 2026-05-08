using ComChienMaDui.Models;using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ComChienMaDui.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    // Inject JwtSettings thông qua IOptions
    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    // Tạo token JWT dựa trên thông tin của user
    public string GenerateToken(User user)
    {
        // Tạo các claim cho token, bao gồm thông tin cơ bản của user
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        // Tạo khóa bảo mật từ chuỗi key trong JwtSettings
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key)
        );

        // Tạo thông tin đăng ký ký (signing credentials) sử dụng thuật toán HMAC SHA256
        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        // Tạo token JWT với các claim, thời gian hết hạn và thông tin ký
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                _jwtSettings.ExpireMinutes
            ),
            signingCredentials: creds
        );

        // Chuyển token thành chuỗi và trả về
        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}