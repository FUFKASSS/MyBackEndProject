using Microsoft.EntityFrameworkCore;
using NewMyProject.Data;
using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;
using System.Security.Claims;

namespace NewMyProject.Services
{
    public class UserService : IUserService
    {
        private readonly EfContext _context;
        private readonly ITokenService _tokenService;

        public EfContext Context { get; }

        
        public UserService(EfContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        
        public async Task<ResponseStatus> RegisterUser(long PhoneNumber, 
                                                       string UserName, 
                                                       string Password, 
                                                       string Email)
        {
            try
            {
                Profile profile = new Profile
                {
                    Name = UserName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                };
                
                User user = new User
                {
                    PhoneNumber = PhoneNumber,
                    UserName = UserName,
                    Role = "User",
                    Password = BCrypt.Net.BCrypt.HashPassword(Password),
                    Email = Email,
                    Profile = profile
                };
                var duplicateUser = await _context.LoginModels.FirstOrDefaultAsync(x => 
                                                                      x.UserName == user.UserName);
                var duplicateEmail = await _context.LoginModels.FirstOrDefaultAsync(x => 
                                                                      x.Email == user.Email);
                var duplicatePhoneNumber = await _context.LoginModels.FirstOrDefaultAsync(x =>
                                                                      x.PhoneNumber == user.PhoneNumber);
                if(duplicateUser == null && duplicateEmail == null && duplicatePhoneNumber == null)
                {
                    await _context.LoginModels.AddAsync(user);
                    await _context.Profiles.AddAsync(profile);
                    await _context.SaveChangesAsync();
                    return new ResponseStatus
                    {
                        Status = "Success. Account Created!"
                    };
                }
                else
                {
                    return new ResponseStatus
                    {
                        Status = "Sorry. This Account exist!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseStatus
                {
                    Status = ex.Message
                };
            }
        }

        
        public User GetByUsername(string username)
        {
            return _context.LoginModels.FirstOrDefault(u => 
                                        u.UserName == username);
        }


        public async Task<TokenApiDto> Login(string UserName, 
                                             string Password)
        {
            var user = await _context.LoginModels.FirstOrDefaultAsync(u => 
                                                    u.UserName == UserName);
            if (user == null)
            {
                throw new Exception("Invalid name or password");
            }
            
            if (!BCrypt.Net.BCrypt.Verify(Password, user.Password))
            {
                throw new Exception("Invalid credentials");
            }
           
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Email", user.Email),
                new Claim("PhoneNumber", user.PhoneNumber.ToString()),
            };
            
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            
            
            user.RefreshToken = refreshToken; 
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            
            await _context.SaveChangesAsync();
            return new TokenApiDto { AccessToken = accessToken, 
                                    RefreshToken = refreshToken };
        }

        public async Task<ResponseStatus> UpdateUserAndProfile(string id, 
                                                               long PhoneNumber, 
                                                               string ProfileName, 
                                                               string? Password, 
                                                               string Email)
        {
            try
            {
                var upduser = await _context.LoginModels.FirstOrDefaultAsync(x =>
                                                                x.Id == int.Parse(id));
                if (Password != null)
                {
                    upduser.Password = BCrypt.Net.BCrypt.HashPassword(Password);
                }
                upduser.Email = Email;
                var updprofile = await _context.Profiles.FirstOrDefaultAsync(x => 
                                                        x.UserInfoKey == int.Parse(id));
                updprofile.Email = upduser.Email;
                updprofile.Name = ProfileName;
                if (PhoneNumber != null)
                {
                    upduser.PhoneNumber = PhoneNumber;
                    updprofile.PhoneNumber = upduser.PhoneNumber;
                }

                await _context.SaveChangesAsync();
                return new ResponseStatus
                {
                    Status = "Data updated!"
                };
            }
           catch (Exception ex)
            {
                return new ResponseStatus
                {
                    Status = ex.Message
                };
            }
        }
    }
}
