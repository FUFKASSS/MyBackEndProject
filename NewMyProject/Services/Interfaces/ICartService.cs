using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;

namespace NewMyProject.Services.Interfaces
{
    public interface ICartService
    {
        public Task<ResponseStatus> AddToCart(int productId, int weightId, 
                                              int typeId, string userId, 
                                              string profileId, int status = 0);
        public Task<ResponseStatus> DeleteFromCart(int productId, int weightId, 
                                                   int typeId, string profileId);
        public Task<ResponseStatus> IncrementItemInCart(int productId, int weightId, 
                                                        int typeId, string profileId);
        public Task<ResponseStatus> DecrementItemInCart(int productId, int weightId, 
                                                        int typeId, string profileId);
        public Task<List<Order>> GetMyCart(string profileId);
        public Task<ResponseStatus> ChangeCartStatusFromAdmin(int id, int status);
    }
}
