using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewMyProject.Data;
using NewMyProject.DTO;
using NewMyProject.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NewMyProject.Test.IntegrationTests
{
    [TestFixture]
    public class TokenControllerTest 
    {
        private readonly static Profile _profile1 = new Profile
        {
            Id = 123,
            Name = "Sash222a1",
            Email = "Sasha1@mail.ru",
            PhoneNumber = 79856734112,
        };
        private static User _user1 = new User
        {
            Id = 123,
            UserName = "Sash222a1",
            Email = "Sasha1@mail.ru",
            PhoneNumber = 79856734112,
            Password = BCrypt.Net.BCrypt.HashPassword("YesofCourse192745"),
            Role = "Admin",
            Profile = _profile1,
        };
        private readonly static LoginDto loginform = new LoginDto
        {
            UserName = _user1.UserName,
            Password = "YesofCourse192745"
        };
        private readonly HttpClient _client;
        public TokenControllerTest()
        {
            // настройка тестируемого приложения
            var factory = new WebApplicationFactory<Program>()
                                                          .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var dbContextDescriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<EfContext>));
                    services.Remove(dbContextDescriptor);

                    services.AddDbContext<EfContext>(options =>
                    {
                        options.UseInMemoryDatabase("Develop_db");
                    });
                });
            });
            _client = factory.CreateClient();
            EfContext dbContext =
            factory.Services.CreateScope().ServiceProvider.GetService<EfContext>();
            dbContext.LoginModels.AddAsync(_user1);
            dbContext.Profiles.AddAsync(_profile1);
            dbContext.SaveChangesAsync();
        }
        public async Task<TokenApiDto> LoginInAccountAndReturnToken(LoginDto login)
        {
            var jsonAuth = JsonConvert.SerializeObject(login, Formatting.Indented);
            HttpContent content = new StringContent(jsonAuth, Encoding.UTF8, "application/json");
            var ResponseUser = await _client.PostAsync("Api/Auth/LoginInAccount", content);
            var result = await ResponseUser.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<TokenApiDto>(result);
            return container;
        }
        [Test]
        public async Task RevokeToken_RevokeUserRefreshToken_ShouldReturnNotNull()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                        LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            // отправка POST-запроса
            var json = JsonConvert.SerializeObject(null, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Api/Token/RevokeToken", content);
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task RefreshToken_UpdateRefreshUserToken_ShouldReturnNotNull()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                            LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            // отправка POST-запроса
            var json = JsonConvert.SerializeObject(LoginInAccountAndReturnToken(loginform).Result, 
                                                                                   Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Api/Token/RefreshToken", content);
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
