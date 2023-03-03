using Microsoft.EntityFrameworkCore;
using Moq;
using NewMyProject.Data;
using NewMyProject.Entities;
using NewMyProject.Services;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NewMyProject.Test
{
    public class UserServiceTest : UnitTestBase
    {
        private readonly EfContext _context;
        private Mock<ITokenService> _tokenService = new Mock<ITokenService>();

        private static readonly Profile _profile1 = new Profile
        {
            Id = 1,
            Name = "Maximkasa",
            Email = "Maximkasa@mail.ru",
            PhoneNumber = 798567347,
        };

        private static readonly User _user1 = new User
        {
            Id = 1,
            UserName = "Maximkasa",
            Email = "Maximkasa@mail.ru",
            PhoneNumber = 798567347,
            Password = BCrypt.Net.BCrypt.HashPassword("HelloWorldsss@s2"),
            Profile = _profile1,
            Role = "User",
        };

        public UserServiceTest()
        {
            _context = CreateInMemoryContext(x => x.AddRange(_user1, 
                                                            _profile1));
        }

        [Fact]
        public async Task GetAccount_GetAccountByUserName_ReturnsRowDataAsync()
        {
            var service = new UserService(_context, _tokenService.Object);

            var GetUser = await service.GetByUsername(_user1.UserName);

            Assert.NotNull(GetUser);
        }

        [Fact]
        public async Task Register_CreateAccount_ReturnsRowDataAsync()
        {
            var service = new UserService(_context, _tokenService.Object);
            await service.RegisterUser(7987212352, 
                                      "Nicky", 
                                      "Nicky123", 
                                      "Nicky132@mail.ru");

            var foundUser = await _context.LoginModels.FirstOrDefaultAsync(x => 
                                                    x.PhoneNumber == 7987212352);
            var foundProfile = await _context.Profiles.FirstOrDefaultAsync(x => 
                                                    x.PhoneNumber == 7987212352);

            Assert.NotNull(foundUser);
            Assert.NotNull(foundProfile);
            Assert.Equal(foundUser.Email, foundProfile.Email);
            Assert.Equal(foundUser.UserName, foundProfile.Name);
            Assert.Equal(foundUser.PhoneNumber, foundProfile.PhoneNumber);
        }

        [Fact]
        public async Task UpdateAccount_UpdateUserAndProfile_ReturnsRowDataAsync()
        {
            var service = new UserService(_context, _tokenService.Object);

            await service.UpdateUserAndProfile(_user1.Id.ToString(), 
                                               7985673472, 
                                               "Sandy", 
                                               "HelloWorldsss@s2", 
                                               "Maximkasa@mail.ru");

            var updUserName = await _context.LoginModels.Where(x => x.Id == 1)
                                                        .Where(x => x.PhoneNumber == 7985673472)
                                                        .FirstOrDefaultAsync();

            Assert.NotEqual(updUserName, _user1);
        }

        [Fact]
        public async Task Login_LoginInAccount_ReturnsRowDataAsync()
        {

            var service = new UserService(_context, _tokenService.Object);

            var register = await service.RegisterUser(79872123512, 
                                                      "Nicky2as3", 
                                                      "Nicky12as3", 
                                                      "Nicky132@mail.ru");

            var find = await _context.LoginModels.FirstOrDefaultAsync(x => 
                                              x.PhoneNumber == 79872123512);
            var result = await service.Login("Nicky2as3", "Nicky12as3");

            Assert.NotNull(register);
            Assert.NotNull(result);
        }
    }
}
