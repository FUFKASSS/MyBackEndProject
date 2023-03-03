using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewMyProject.Common;
using NewMyProject.DTO;
using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;
using NewMyProject.Services;

namespace NewMyProject.Controllers
{
    [ModelValidator]
    [ModelValidatorAttribute]
    [Route("Api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService; 

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("RegisterUser")]
        public async Task<ResponseStatus> RegisterUser([FromBody] RegisterDto  dto) 
        {
            return await _userService.RegisterUser(dto.PhoneNumber, dto.UserName, 
                                                   dto.Password, dto.Email);
        }

        [HttpPost("LoginInAccount")]
        public async Task<TokenApiDto> Login([FromBody] LoginDto dto)
        {
            var LoginMethod = _userService.Login(dto.UserName, dto.Password);
            var cookieOptions = new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                IsEssential = true,
                HttpOnly = true,
                Secure = true,
            };

            HttpContext.Response.Cookies.Append("jwtRefreshToken", 
                                                (await LoginMethod).RefreshToken, 
                                                cookieOptions);
            return await LoginMethod;
        }

        [HttpPut("UpdateUserAndProfile"), Authorize(AuthenticationSchemes = "Bearer ")]
        public async Task<ResponseStatus> UpdateUserAndProfile([FromBody]
                                                               UpdateUserAndProfileDto upd)
        {
            var id = HttpContext.Items["userId"];
            return await _userService.UpdateUserAndProfile(id.ToString(),
                                                           upd.PhoneNumber,
                                                           upd.UserName,
                                                           upd.Password,
                                                           upd.Email);
        }
    }
}
