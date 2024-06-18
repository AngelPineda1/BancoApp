using BancoAPI.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BancoAPI.Helpers
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration configuration;
        public JwtTokenGenerator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string GetTokenUser(Usuarios user, string role)
        {
            var Iss = configuration["JwtSettings:Issuer"];
            var aud = configuration["JwtSettings:Audience"];
            var key = configuration["JwtSettings:SecretKey"];
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, user.Nombre));
            claims.Add(new Claim(ClaimTypes.Email, "user@gmail.com"));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, Iss));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, aud));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddMinutes(5).ToString()));

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = Iss,
                Audience = aud,
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddDays(7),
                NotBefore = DateTime.Now,
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        public string GetTokenCajas(Cajas cajas, string role)
        {
            var Iss = configuration["JwtSettings:Issuer"];
            var aud = configuration["JwtSettings:Audience"];
            var key = configuration["JwtSettings:SecretKey"];
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, cajas.Nombre));
            claims.Add(new Claim(ClaimTypes.Email, cajas.Username));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, cajas.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, Iss));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, aud));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddMinutes(5).ToString()));

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = Iss,
                Audience = aud,
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddDays(7),
                NotBefore = DateTime.Now,
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}
