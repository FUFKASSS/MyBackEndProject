using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewMyProject.DTO;
using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;
using NewMyProject.Services;

namespace NewMyProject.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }
        

        [HttpPost("AddProduct"), 
         Authorize(AuthenticationSchemes = "Bearer ", Roles = "Admin") ]
        public async Task<Product> CreateProduct([FromBody] Product product)
        {
            return await _service.CreateProduct(product);
        }

        [HttpGet("QueryAllProducts")]
        public async Task<List<Product>> QueryGetAll(
                 [FromQuery(Name = "search")] string? search,
                 [FromQuery(Name = "sort")] string? sort,
                 [FromQuery(Name = "direction")] string? direction,
                 [FromQuery(Name = "category")] int? category)
        {
            return await _service.QueryGetAllProducts(search, sort,
                                        direction, category);
        }

        
        [HttpGet("GetProductById/{id}")]
        public async Task<Product> GetProductById([FromRoute] int id)
        {
            return await _service.GetProductById(id);
        }

        [HttpPut("UpdateProduct"), 
         Authorize(AuthenticationSchemes = "Bearer ", Roles = "Admin")]
        public async Task<ResponseStatus> UpdateProduct([FromBody] UpdateProductDto newProduct)
        {
            return await _service.UpdateProduct(newProduct.Id, newProduct.Title, 
                                                newProduct.Description, newProduct.Category, 
                                                newProduct.Rating, newProduct.Price);
        }

        [HttpPut("UpdateType"), 
         Authorize(AuthenticationSchemes = "Bearer ", Roles = "Admin")]
        public async Task<ResponseStatus> UpdateType([FromBody] TypeProductDto Type)
        {
            return await _service.UpdateType(Type.Id, Type.Name, Type.Image, Type.AdditionPrice);
        }

        [HttpPut("UpdateWeight"), 
         Authorize(AuthenticationSchemes = "Bearer ", Roles = "Admin")]
        public async Task<ResponseStatus> UpdateWeight([FromBody] UpdateWeightDto Weight)
        {
            return await _service.UpdateWeight(Weight.Id, Weight.Name, Weight.AdditionPrice);
        }

        [HttpDelete("DeleteProduct/{productId}"), 
         Authorize(AuthenticationSchemes = "Bearer ", Roles = "Admin")]
        public async Task<ResponseStatus> DeleteProduct(int productId)
        {
           return await _service.DeleteProduct(productId);
        }
    }
}
