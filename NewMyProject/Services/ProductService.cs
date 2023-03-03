using Microsoft.EntityFrameworkCore;
using NewMyProject.Data;
using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;

namespace NewMyProject.Services
{
    public class ProductService : IProductService
    {
        private readonly EfContext _context;

        public ProductService(EfContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetProductById(int Id)
        {
            var productById = await _context.Products.Include(x => x.Types)
                                                     .Include(x => x.Weights)
                                                     .FirstOrDefaultAsync(x => x.Id == Id);
            if (productById == null)
            {
                throw new Exception("Такого Id не существует");
            }
            else
            {
                return productById;
            }
        }

        public async Task<List<Product>> QueryGetAllProducts(string? search, string? sort, 
                                                 string? direction, int? category)
        {
            var query = _context.Products.Include(x => x.Weights).Include(x => x.Types).AsSingleQuery();
            if(search is null && sort is null && direction is null && category is null)
            {
                return await query.ToListAsync().ConfigureAwait(false);
            }
            else
            {
                query = setCategoryFilter(query, category);
                query = setSortingStrategy(query, sort, direction);
                query = setSearchCondition(query, search);

                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public async Task<ResponseStatus> DeleteProduct(int productId)
        {
            try
            {
                var Delete = await _context.Products.FirstOrDefaultAsync(x => 
                                                                         x.Id == productId);
                var DeleteType = await _context.TypeProducts.FirstOrDefaultAsync(x => 
                                                                                 x.ProductId == productId);
                var DeleteWeight = await _context.WeightProducts.FirstOrDefaultAsync(x => 
                                                                                 x.ProductId == productId);
                _context.Products.Remove(Delete);
                _context.TypeProducts.Remove(DeleteType);
                _context.WeightProducts.Remove(DeleteWeight);
               await _context.SaveChangesAsync();
                return new ResponseStatus { Status = "Product Deleted" };
            }
            catch (Exception ex)
            {
                return new ResponseStatus { Status = ex.Message };
            }
        }

        public async Task<ResponseStatus> UpdateProduct(int Id, string title, string description, 
                                                        int category, int rating, decimal price)
        {
            try
            {
                Product updproduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == Id);
                updproduct.Title = title;
                updproduct.Description = description;
                updproduct.Category = category;
                updproduct.Rating = rating;
                updproduct.Price = price;
                await _context.SaveChangesAsync();

                return new ResponseStatus { Status = "Product Updated" };
            }
            catch(Exception ex)
            {
                return new ResponseStatus { Status = ex.Message };
            }
        }



        public async Task<ResponseStatus> UpdateType(int id, string name, string image, decimal additionPrice)
        {
            try
            {
                var updtype = await _context.TypeProducts.FirstOrDefaultAsync(x => x.Id == id);
                updtype.Name = name;
                updtype.Image = image;
                updtype.AdditionPrice = additionPrice;
                await _context.SaveChangesAsync();
                return new ResponseStatus { Status = "Type Updated" };
            }
            catch (Exception ex)
            {
                return new ResponseStatus { Status = ex.Message };
            }
        }

        public async Task<ResponseStatus> UpdateWeight(int id, string name, decimal additionPrice)
        {
            try
            {
                var updweight = await _context.WeightProducts.FirstOrDefaultAsync(x => x.Id == id);
                updweight.Name = name;
                updweight.AdditionPrice = additionPrice;
                await _context.SaveChangesAsync();
                return new ResponseStatus { Status = "Weight Updated" };
            }
            catch (Exception ex)
            {
                return new ResponseStatus { Status = ex.Message };
            }
        }

        private IQueryable<Product> setSearchCondition(IQueryable<Product> query, string? search)
        {
            if (search == null) return query;
            return query.Where(p => p.Title.Contains(search) || 
                               p.Description.Contains(search));
        }

        private IQueryable<Product> setSortingStrategy(IQueryable<Product> query, string? sort, string? direction)
        {
            if (sort != null)
            {
                switch (sort)
                {
                    case "price": query = setSortingKey(query, p => p.Price, direction); break;
                    case "rating": query = setSortingKey(query, p => p.Rating, direction); break;
                    case "alphabet": query = setSortingKey(query, p => p.Title, direction); break;
                }
            }

            return query;
        }

        private IQueryable<Product> setSortingKey(IQueryable<Product> query, Func<Product, object> callback, string? direction)
        {
            if (direction == "asc") return (IQueryable<Product>)query.OrderBy(callback);
            return (IQueryable<Product>)query.OrderByDescending(callback);
        }

        private IQueryable<Product> setCategoryFilter(IQueryable<Product> query, int? category)
        {
            if (category == null) return query;
            return query.Where(x => x.Category == category);
        }
    }
}
