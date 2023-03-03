using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;
using System.Security.Claims;

namespace NewMyProject.Services
{
    public interface ITokenService
    {
       public string GenerateAccessToken(IEnumerable<Claim> claims);
       public string GenerateRefreshToken();
       public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
       public Task<ResponseStatus> RevokeToken(string UserName);
       public Task<TokenApiDto> GetNewAccessTokenWhenItExpired(string AccessToken, 
                                                               string RefreshToken);
    }
}
