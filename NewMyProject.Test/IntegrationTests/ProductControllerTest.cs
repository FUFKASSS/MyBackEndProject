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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NewMyProject.Test.IntegrationTests
{
    [TestFixture]
    public class ProductControllerTest
    {
        private readonly static Product _product1 = new Product
        {
            Id = 1,
            Title = "Soup",
            Description = "Soup",
            Category = 1,
            Rating = 1,
            Price = 100,
            Weights = new List<WeightProduct>
            {
                _weight1
            },
            Types = new List<TypeProduct>
            {
                _typeProduct1
            }
        };
        private static WeightProduct _weight1 = new WeightProduct
        {
            Id = 121,
            Name = "100",
            AdditionPrice = 0,
            ProductId = _product1.Id
        };
        private static TypeProduct _typeProduct1 = new TypeProduct
        {
            Id = 121,
            Name = "BulkoSup",
            AdditionPrice = 200,
            Image = "image",
            ProductId = _product1.Id
        };
        private readonly static Product _product2 = new Product
        {
            Id = 2,
            Title = "Harcho",
            Description = "Harcho",
            Category = 2,
            Rating = 2,
            Price = 200,
            Weights = new List<WeightProduct>
            {
                new WeightProduct
                {
                    Name = "200",
                    AdditionPrice = 0
                },
                new WeightProduct
                {
                    Name = "400",
                    AdditionPrice = 300
                }
            },
            Types = new List<TypeProduct>
            {
                new TypeProduct
                {
                    Name = "BulkoSoup",
                    Image = "image",
                    AdditionPrice = 250
                },
                new TypeProduct
                {
                    Name = "Container",
                    Image = "image",
                    AdditionPrice = 0
                }
            }
        };
        private readonly static Product _product3 = new Product
        {
            Id = 3,
            Title = "Borch",
            Description = "Borch",
            Category = 3,
            Rating = 3,
            Price = 250,
            Weights = new List<WeightProduct>
            {
                new WeightProduct
                {
                    Id = 102,
                    Name = "300",
                    AdditionPrice = 0,
                    ProductId = 3
                },
                new WeightProduct
                {
                    Id = 103,
                    Name = "500",
                    AdditionPrice = 350,
                    ProductId = 3
                }
            },
            Types = new List<TypeProduct>
            {
                new TypeProduct
                {
                    Id = 104,
                    Name = "BreadSoup",
                    Image = "image",
                    AdditionPrice = 300,
                    ProductId = 3
                },
                new TypeProduct
                {
                    Id = 105,
                    Name = "Conteiner",
                    Image = "image",
                    AdditionPrice = 0,
                    ProductId = 3
                }
            }
        };

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
        public ProductControllerTest()
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
            dbContext.Products.AddRangeAsync(_product2, _product1);
            dbContext.WeightProducts.AddAsync(_weight1);
            dbContext.TypeProducts.AddAsync(_typeProduct1);
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
        public async Task GetProductById_ExistProductAvaliable_ShouldReturnNotNull()
        {
            //Отправка GET-запроса
            var response = await _client
                                        .GetAsync($"Api/Product/GetProductById/{_product2.Id}");

            var responseProduct = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<Product>(responseProduct);
            // проверка результатов
            Assert.NotNull(container);
            Assert.AreEqual(_product2.Id, container.Id);
        }

        [Test]
        public async Task QueryGetAll_FilterExistProductAvaliable_ShouldReturnNotNull()
        {
            //Отправка GET-запроса
            var QueryControler = "Api/Product/QueryAllProducts?sort=Price&direction=asc&category=1&search=Soup";
            HttpResponseMessage response = await _client
                                                    .GetAsync(QueryControler);

            var responseProduct =  await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<List<Product>>(responseProduct);
            // проверка результатов
            Assert.NotNull(container);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task CreateProduct_ProductAvaliable_ShouldReturnNotNull()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                            LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            // отправка POST-запроса
            var json = JsonConvert.SerializeObject(_product3, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Api/Product/AddProduct", content);

            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<Product>(result);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(container);
        }

        [Test]
        public async Task UpdateProduct_ProductAvaliableAndUpdate_ShouldReturnNotNull()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                        LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            var updProdut = new UpdateProductDto()
            {
                Id = 4,
                Title = "HelloWorld",
                Description = "HelloWorld",
                Category = 19,
                Rating = 19,
                Price = 19
            };
            // отправка PUT-запроса
            var json = JsonConvert.SerializeObject(updProdut, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("Api/Product/UpdateProduct", content);
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<ResponseStatus>(result);
            Assert.NotNull(container);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task UpdateType_ProductAvaliableAndUpdate_ShouldReturnNotNull()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                        LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            var updType = new TypeProductDto
            {
                Id = 102,
                Name = "Sssss",
                AdditionPrice = 300,
                Image = "Image"
            };
            // отправка PUT-запроса
            var json = JsonConvert.SerializeObject(updType, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("Api/Product/UpdateType", content);
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<ResponseStatus>(result);
            Assert.NotNull(container);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task UpdateWeight_ProductAvaliableAndUpdate_ShouldReturnNotNull()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                            LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            var updWeight = new UpdateWeightDto
            {
                Id = 121,
                Name = "Sssss",
                AdditionPrice = 300
            };
            // отправка PUT-запроса
            var json = JsonConvert.SerializeObject(updWeight, Formatting.Indented);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("Api/Product/UpdateWeight", content);
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<ResponseStatus>(result);
            Assert.NotNull(container);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task DeleteProduct_ProductNotAvaliableAndUpdate_ShouldReturnDeleted()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                                           LoginInAccountAndReturnToken(loginform).Result.AccessToken);
            // отправка DELETE-запроса
            var response = await _client.DeleteAsync($"Api/Product/DeleteProduct/{_product1.Id}");
            // проверка результатов
            var result = await response.Content.ReadAsStringAsync();
            var container = JsonConvert.DeserializeObject<ResponseStatus>(result);
            Assert.NotNull(container);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
