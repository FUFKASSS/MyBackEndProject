using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewMyProject.Common;
using NewMyProject.Data;
using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;
using NewMyProject.Services;

namespace NewMyProject.Controllers
{
    [ModelValidator]
    [ModelValidatorAttribute]
    [Route("Api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly EfContext _efContext;
        private readonly ITokenService _tokenService;

        public TokenController(EfContext efContext, 
                               ITokenService tokenService)
        {
            _efContext = efContext ?? 
                throw new ArgumentNullException(nameof(_efContext));
            _tokenService = tokenService ?? 
                      throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost("RefreshToken"), 
         Authorize(AuthenticationSchemes = "Bearer ")]
        public async Task<TokenApiDto> RefreshToken([FromBody] TokenApiDto tokenApidto)
        {
           var RefreshToken = Request.Cookies["jwtRefreshtoken"];
           return await _tokenService.GetNewAccessTokenWhenItExpired(tokenApidto.AccessToken, 
                                                                     RefreshToken);
        }

        [HttpPost("RevokeToken"), 
         Authorize(AuthenticationSchemes = "Bearer ")]
        public async Task<ResponseStatus> Revoke()
        {
            var username = User.Identity.Name;

            await _tokenService.RevokeToken(username);
            Response.Cookies.Delete("jwtRefreshtoken");

            return new ResponseStatus
            {
                Status = "Token revoked"
            };
        }
    }
}
