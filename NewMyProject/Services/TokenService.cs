using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewMyProject.Config;
using NewMyProject.Data;
using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NewMyProject.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly EfContext _context;

        public TokenService(IOptions<JwtConfig> jwtConfig, EfContext context)
        {
            _jwtConfig = jwtConfig.Value;
            _context = context;
        }

        private byte[] GetSecretKey()
        {
            return Encoding.UTF8.GetBytes(_jwtConfig.SecretKey);
        }
        
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(GetSecretKey());
            var signinCredentials = new SigningCredentials(secretKey, 
                                        SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtConfig.ExpiresInMinutes),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }

        
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<ResponseStatus> RevokeToken(string UserName)
        {
            try
            {
                var user = _context.LoginModels.SingleOrDefault(u => 
                                                u.UserName == UserName);
                user.RefreshToken = null;
                await _context.SaveChangesAsync();
                return new ResponseStatus
                {
                    Status = "Success, token revoked"
                };
            }
            catch(Exception ex)
            {
                return new ResponseStatus
                {
                    Status = ex.Message
                };
            }
        }

        public async Task<TokenApiDto> GetNewAccessTokenWhenItExpired(string AccessToken, 
                                                                      string RefreshToken)
        {

            var principal = GetPrincipalFromExpiredToken(AccessToken);
            
            var username = principal.Identity.Name;
            var user = _context.LoginModels.SingleOrDefault(u => u.UserName == username);
            if (user == null || user.RefreshToken != RefreshToken || 
                                                     user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return null;
            }
            var newAccessToken = GenerateAccessToken(principal.Claims);
            await _context.SaveChangesAsync();
            return new TokenApiDto
            {
                AccessToken = AccessToken,
                RefreshToken = RefreshToken,
            };
        }

        
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true, 
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(GetSecretKey()),
                ValidateLifetime = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidAudience = _jwtConfig.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                                                            out SecurityToken securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.
                                                         Equals(SecurityAlgorithms.HmacSha256,
                                                         StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");
                return principal;
            }
            catch(Exception ex){
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
