using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NewMyProject.Config;
using NewMyProject.Data;
using NewMyProject.Entities;
using NewMyProject.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace NewMyProject.Test
{
    public class TokenServiceTest : UnitTestBase
    {
        private readonly EfContext _context;
        private readonly IConfigurationRoot _config;

        private static readonly List<Claim> _userClaims = new List<Claim>
        {
            new Claim("id", "1"),
            new Claim(ClaimTypes.Name, "Maximkaasa"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("Email", "Maximkasaa@mail.ru"),
            new Claim("PhoneNumber", "794512237347"),
        };

        private static readonly User _user = new User
        {
            Id = 1,
            UserName = "Maximkaasa",
            Email = "Maximkasaa@mail.ru",
            PhoneNumber = 794512237347,
            Password = BCrypt.Net.BCrypt.HashPassword("HelloWorldsss@s2a"),
            Role = "Admin",
            RefreshToken = "ad21AAsd1_dm@#453(71"
        };

        public TokenServiceTest()
        {
            _context = CreateInMemoryContext(x => x.AddRange(_user));
            _config = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json")
                    .Build();
        }

        [Fact]
        public  void RefreshToken_GenerateRefreshToken_ReturnsRowDataAsync()
        {
            var optionValue = _config.GetSection("Jwt").Get<JwtConfig>();
            var options = Options.Create<JwtConfig>(optionValue);
            var service = new TokenService(options, _context);

            var refreshtoken = service.GenerateRefreshToken();

            Assert.NotNull(refreshtoken);
        }

        [Fact]
        public void AccessToken_GenerateAccessToken_ReturnsRowDataAsync()
        {
            var optionValue = _config.GetSection("Jwt").Get<JwtConfig>();
            var options = Options.Create<JwtConfig>(optionValue);
            var service = new TokenService(options, _context);

            var accestoken = service.GenerateAccessToken(_userClaims);

            Assert.NotNull(accestoken);
        }

        [Fact]
        public void NewAccessToken_GetNewAccessTokenWhenItExpired_ReturnsRowDataAsync()
        {
            var optionValue = _config.GetSection("Jwt").Get<JwtConfig>();
            var options = Options.Create<JwtConfig>(optionValue);
            var service = new TokenService(options, _context);

            var accestoken = service.GenerateAccessToken(_userClaims);
            var refreshtoken = service.GenerateRefreshToken();
            var token = service.GetNewAccessTokenWhenItExpired(accestoken, 
                                                               refreshtoken);

            Assert.NotNull(token);
        }

        [Fact]
        public void GetPrincipal_GetPrincipalFromExpiredToken_ReturnsRowDataAsync()
        {
            var optionValue = _config.GetSection("Jwt").Get<JwtConfig>();
            var options = Options.Create<JwtConfig>(optionValue);
            var service = new TokenService(options, _context);

            var accestoken = service.GenerateAccessToken(_userClaims);
            var token = service.GetPrincipalFromExpiredToken(accestoken);

            Assert.NotNull(token);
        }

        [Fact]
        public void RevokeToken_RevokeRefreshToken_ReturnsRowDataAsync()
        {
            var optionValue = _config.GetSection("Jwt").Get<JwtConfig>();
            var options = Options.Create<JwtConfig>(optionValue);
            var service = new TokenService(options, _context);

            var token = service.RevokeToken(_user.UserName);

            var updtoken = _context.LoginModels.FirstOrDefault(x => 
                                       x.RefreshToken == _user.RefreshToken);

            Assert.Null(updtoken);
        }

    }
}
