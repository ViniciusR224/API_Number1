using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Number1.Services.Jwt_Service
{
    public class JwtGeneratorSevice : IJwtService
    {
        public IConfiguration Configuration { get; }
        public JwtGeneratorSevice(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role,"User"),
              
                
            };
            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"]));
            var SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = Configuration["Jwt:Issuer"],
                Audience = Configuration["Jwt:Audience"],
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = SigningCredentials,
                Subject = new ClaimsIdentity(claims)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString=tokenHandler.WriteToken(token);
            return tokenString;            
        }
    }
}
