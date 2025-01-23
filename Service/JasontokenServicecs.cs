using Microsoft.IdentityModel.Tokens;
using MVC_Studio.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MVC_Studio.Service
{
    public class JasontokenServicecs:IJwtoken
    {

        private readonly string _secretkey;
        public JasontokenServicecs(IConfiguration configuration)
        {
            _secretkey = configuration["Jwt:secretkey"] ?? throw new InvalidOperationException("Failed to secret key");
        }

        public string Createtoken(int id, string email)
        {

            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretkey);
            var tokendescriptot = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Email,email)

                    ]),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenhandler.CreateToken(tokendescriptot);
            return tokenhandler.WriteToken(token);


        }

        public int VerifyToken(string token)
        {
            try 
            {

                var tokenhandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretkey);

                var valiadateparameter = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenhandler.ValidateToken(token, valiadateparameter, out var validatedToken);
                var UserId = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (UserId != null && int.TryParse(UserId.Value, out var userId))
                {
                    return userId;
                }
                else
                {
                    throw new Exception("User ID not found or invalid in token.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("server error", ex);
                    }
        }
    }
}
