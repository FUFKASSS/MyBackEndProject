using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;
using System.Security.Claims;

namespace NewMyProject.Services
{
    public interface ITokenService
    {
       public Task<string> GenerateAccessToken(IEnumerable<Claim> claims);
       public Task<string> GenerateRefreshToken();
       public Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
       public Task<ResponseStatus> RevokeToken(string UserName);
       public Task<TokenApiDto> GetNewAccessTokenWhenItExpired(string AccessToken, 
                                                               string RefreshToken);
    }
}
