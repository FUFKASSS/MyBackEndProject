using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewMyProject.Common;
using NewMyProject.DTO;
using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;
using NewMyProject.Services.Interfaces;

namespace NewMyProject.Controllers
{
    [ModelValidator]
    [ModelValidatorAttribute]
    [Route("Api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("AddToCart"),
         Authorize(AuthenticationSchemes = "Bearer ")]
        public async Task<ResponseStatus> AddToCart([FromBody] CartDto cart)
        {
            var userId = HttpContext.Items["userId"];
            var profileId = HttpContext.Items["profileId"];
            return await _cartService.AddToCart(cart.productId,
                                                cart.weightId,
                                                cart.typeId,
                                                userId.ToString(),
                                                profileId.ToString());
        }

        [HttpGet("GetMyCart"),
         Authorize(AuthenticationSchemes = "Bearer ")]
        public async Task<List<Order>> GetMyCart()
        {
            var profileId = HttpContext.Items["profileId"];
            return await _cartService.GetMyCart(profileId.ToString());
        }

        [HttpPost("IncrementItemInCart"),
         Authorize(AuthenticationSchemes = "Bearer ")]
        public async Task<ResponseStatus> IncrementItemInCart([FromBody] CartDto cart)
        {
            var profileId = HttpContext.Items["profileId"];
            return await _cartService.IncrementItemInCart(cart.productId,
                                                          cart.weightId,
                                                          cart.typeId,
                                                          profileId.ToString());
        }

        [HttpPost("DecrementItemInCart"),
         Authorize(AuthenticationSchemes = "Bearer ")]
        public async Task<ResponseStatus> DecrementItemInCart([FromBody] CartDto cart)
        {
            var profileId = HttpContext.Items["profileId"];
            return await _cartService.DecrementItemInCart(cart.productId,
                                                          cart.weightId,
                                                          cart.typeId,
                                                          profileId.ToString());
        }

        [HttpDelete("DeleteFromCart/{productId}&{weightId}&{typeId}"),
         Authorize(AuthenticationSchemes = "Bearer ")]
        public async Task<ResponseStatus> DeleteFromCart(int productId, int weightId, int typeId)
        {
            var profileId = HttpContext.Items["profileId"];
            return await _cartService.DeleteFromCart(productId,
                                                     weightId,
                                                     typeId,
                                                     profileId.ToString());
        }

        [HttpPut("ChangeOrderStatusFromAdmin"),
         Authorize(AuthenticationSchemes = "Bearer ", Roles = "Admin")]
        public async Task<ResponseStatus> ChangeCartStatusFromAdmin([FromBody] CartDtoForAdmin cart)
        {
            return await _cartService.ChangeCartStatusFromAdmin(cart.id, cart.status);
        }
    }
}
