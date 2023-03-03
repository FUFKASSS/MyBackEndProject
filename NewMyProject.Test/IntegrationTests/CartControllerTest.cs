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
    public class CartControllerTest
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
            UserName = _user1.UserName,
            Password = "YesofCourse192745"
        };
        private readonly static CartDto cart = new CartDto
        {
            productId = 1,
            typeId = 121,
            weightId = 121
        };
        private readonly static CartDtoForAdmin cartUpdateAdmin = new CartDtoForAdmin
        {
            id = 1,
            status = 2,
        };
        public CartControllerTest()
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
            dbContext.LoginModels.AddAsync(_user1);
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
        public async Task<ResponseStatus> AddInCart()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                        LoginInAccountAndReturnToken(loginform).Result.AccessToken);

            // отправка POST-запроса
            var json = JsonConvert.SerializeObject(cart, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Api/Cart/AddToCart", content);
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<ResponseStatus>(result);
            return container;
        }



        [Test]
        public async Task AddInCart_ProductInUserCart_ShouldReturnNotNull()
        {
            var result = await AddInCart();
            Assert.NotNull(result.Status);
        }

        [Test]
        public async Task IncrementItemInCart_IncrementProductInUserCart_ShouldReturnNotNull()
        {
            await AddInCart();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                        LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            // отправка POST-запроса
            var json = JsonConvert.SerializeObject(cart, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Api/Cart/IncrementItemInCart", content);
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<ResponseStatus>(result);
            Assert.NotNull(container);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task DecrementItem_DecrementProductInUserCart_ShouldReturnNotNull()
        {
            await AddInCart();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                    LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            // отправка POST-запроса
            var json = JsonConvert.SerializeObject(cart, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Api/Cart/DecrementItemInCart", content);
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<ResponseStatus>(result);
            Assert.NotNull(container);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task GetCart_GetMyCart_ShouldReturnNotNull()
        {
            await AddInCart();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                        LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            // отправка GET-запроса
            var response = await _client.GetAsync("Api/Cart/GetMyCart");
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task DeleteItem_DeleteProductInUserCart_ShouldReturnNotNull()
        {
            await AddInCart();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                        LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            // отправка DELETE-запроса
            var response = await _client.DeleteAsync("Api/Cart/DeleteFromCart/1&121&121");
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<ResponseStatus>(result);
            Assert.NotNull(container);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task ChangeCartStatusFromAdmin_ChangeCartStatusToOrderStatus_ShouldReturnNotNull()
        {
            await AddInCart();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                        LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            // отправка PUT-запроса
            var json = JsonConvert.SerializeObject(cartUpdateAdmin, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("Api/Cart/ChangeOrderStatusFromAdmin", content);
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<ResponseStatus>(result);
            Assert.NotNull(container);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
