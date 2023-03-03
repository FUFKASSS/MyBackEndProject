using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;

namespace NewMyProject.Services
{
    public interface IUserService
    {
        public Task<ResponseStatus> RegisterUser(long PhoneNumber, 
                                                 string ProfileName, 
                                                 string Password, 
                                                 string Email);
        public Task<User> GetByUsername(string username);
        public Task<TokenApiDto> Login(string UserName, 
                                       string Password);
        public Task<ResponseStatus> UpdateUserAndProfile(string id, 
                                                         long PhoneNumber, 
                                                         string UserName, 
                                                         string? Password, 
                                                         string Email);
    }
}
