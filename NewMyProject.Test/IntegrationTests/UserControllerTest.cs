using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewMyProject.Data;
using NewMyProject.DTO;
using NewMyProject.DTO.responseDto;
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
    public class UserControllerTest
    {
        private readonly HttpClient _client;
        private readonly static Profile _profile2 = new Profile
        {
            Id = 123,
            Name = "Sash222a1",
            Email = "Sasha1@mail.ru",
            PhoneNumber = 79856734112,
        };
        private static User _user1 = new User
        {
            Id = 123123,
            UserName = "Sash222a",
            Email = "Sasha@mail.ru",
            PhoneNumber = 79856734112,
            Password = BCrypt.Net.BCrypt.HashPassword("YesofCourse19274")
        };
        private static User _user2 = new User
        {
            Id = 123,
            UserName = "Sash222a1",
            Email = "Sasha1@mail.ru",
            PhoneNumber = 79856734112,
            Password = BCrypt.Net.BCrypt.HashPassword("YesofCourse192745"),
            Role = "Admin",
            Profile = _profile2,
        };
        private readonly static LoginDto loginform = new LoginDto
        {
            UserName = _user2.UserName,
            Password = "YesofCourse192745"
        };
        public UserControllerTest()
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
            // создание HTTP-клиента и добавление заголовка авторизации
            _client = factory.CreateClient();
            EfContext dbContext =
            factory.Services.CreateScope().ServiceProvider.GetService<EfContext>();
            dbContext.LoginModels.AddAsync(_user2);
            dbContext.Profiles.AddAsync(_profile2);
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

        public async Task<ResponseStatus> RegisterAccount(User user)
        {
            var jsonRegister = JsonConvert.SerializeObject(user, Formatting.Indented);
            HttpContent contentRegister = new StringContent(jsonRegister, Encoding.UTF8, "application/json");
            var ResponseRegister = await _client.PostAsync("Api/Auth/RegisterUser", contentRegister);
            var RegisterResult = await ResponseRegister.Content.ReadAsStringAsync();
            var ResponseContainerResult = JsonConvert.DeserializeObject<ResponseStatus>(RegisterResult);
            return ResponseContainerResult;
        }

        [Test]
        public async Task Login_LoginInAccount_ShouldReturnToken()
        {
            var tokens = await LoginInAccountAndReturnToken(loginform);
            Assert.NotNull(tokens);
        }

        [Test]
        public async Task RegisterUser_CreateUser_ShouldReturnMessageCreated()
        {
            var ContainerResult = "Success. Account Created!";
            var result = await RegisterAccount(_user1);
            Assert.NotNull(result.Status, ContainerResult);
        }
        [Test]
        public async Task UpdateUserAndProfile_UpdateUserAccount_ShouldReturnNotNull()
        {
            UpdateUserAndProfileDto upd = new UpdateUserAndProfileDto
            {
                Email = "helloworld@mail.ru",
                Password = "hiworldmy",
                PhoneNumber = 76959487,
                UserName = "jkdkasj@"
            };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                        LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            // отправка PUT-запроса
            var json = JsonConvert.SerializeObject(upd, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("Api/Auth/UpdateUserAndProfile", content);

            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);

        }
    }
}
