using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;

namespace NewMyProject.Services
{
    public interface IProductService
    {
        public Task<Product> CreateProduct(Product product);
        public Task<Product> GetProductById(int Id);
        public List<Product> QueryGetAllProducts(string? search, string? sort, 
                                         string? direction, int? category);
        public Task<ResponseStatus> UpdateProduct(int id, string title, 
                                                  string description, int category, 
                                                  int rating, decimal price);
        public Task<ResponseStatus> UpdateType(int id, string name, 
                                               string image, decimal additionPrice);
        public Task<ResponseStatus> UpdateWeight(int id, string name, decimal additionPrice);
        public Task<ResponseStatus> DeleteProduct(int productId);
    }
}
