using Microsoft.EntityFrameworkCore;
using NewMyProject.Data;
using NewMyProject.Entities;
using NewMyProject.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NewMyProject.Test
{
    public class ProductServiceTest : UnitTestBase
    {
        private readonly EfContext _context;

        private readonly Product _product1 = new Product
        {
            Id = 1,
            Title = "Soup",
            Description = "Soup",
            Category = 1,
            Rating = 1,
            Price = 100,
            Weights = new List<WeightProduct>
            {
                new WeightProduct
                {
                    Name = "100",
                    AdditionPrice = 0
                },
                new WeightProduct
                {
                    Name = "300",
                    AdditionPrice = 200
                }
            },
            Types = new List<TypeProduct>
            {
                new TypeProduct
                {
                    Name = "BulkoSup",
                    Image = "image",
                    AdditionPrice = 200,
                },
                new TypeProduct
                {
                    Name = "Kontainer",
                    Image = "image",
                    AdditionPrice = 0,
                }
            }

        };
        
        public ProductServiceTest()
        {
            _context = CreateInMemoryContext(x => x.AddRange(_product1));
        }

        [Fact]
        public async void GetProduct_GetProductById_ReturnsRowDataAsync()
        {
            var service = new ProductService(_context);
            var product = await service.GetProductById(_product1.Id);

            Assert.NotNull(product);
        }

        [Fact]
        public async void GetProducts_GetQueryGetAllProducts_ReturnsRowDataAsync()
        {
            var service = new ProductService(_context);
            var product =  service.QueryGetAllProducts("Soup", "", "", 1);

            Assert.NotNull(product);
        }

        [Fact]
        public async void AddProduct_CreateProduct_ReturnsRowDataAsync()
        {
            var service = new ProductService(_context);
            var entity = new Product
            {
                Id = 5,
                Title = "Chiki-Briki",
                Description = "Chiki-Briki",
                Category = 23,
                Rating = 23,
                Price = 3001,
                Weights = new List<WeightProduct>
                {
                    new WeightProduct
                    {
                        Name = "200asdas",
                        AdditionPrice = 0
                    },
                    new WeightProduct
                    {
                        Name = "400asdasd",
                        AdditionPrice = 300
                    }
                },
                Types = new List<TypeProduct>
                {
                    new TypeProduct
                    {
                        Name = "Wonrdefullasdasd",
                        Image = "imageasdad",
                        AdditionPrice = 400,
                    },
                    new TypeProduct
                    {
                        Name = "Kontainerasd",
                        Image = "image",
                        AdditionPrice = 0,
                    }
                }
            };

            var response = await service.CreateProduct(entity);
            var found = await _context.Products.FirstOrDefaultAsync(x => 
                                                        x.Id == response.Id);

            Assert.NotNull(response);
            Assert.NotNull(found);
            Assert.Equal(response.Id, found.Id);
            Assert.Equal(entity.Price, found.Price);
            Assert.Equal(entity.Category, found.Category);
            Assert.Equal(entity.Description, found.Description);
            Assert.Equal(entity.Rating, found.Rating);
            Assert.Equal(entity.Types, found.Types);
            Assert.Equal(entity.Weights, found.Weights);
        }

        [Fact]
        public async void UpdateProduct_UpdateProductWithTypesAndWeights_ReturnsRowDataAsync()
        {
            var service = new ProductService(_context);
            var entity = new Product()
            {
                Id = 12,
                Title = "harcho",
                Description = "harcho",
                Category = 5,
                Rating = 5,
                Price = 400,
                Weights = new List<WeightProduct>
                {
                    new WeightProduct
                    {
                        Id = 10,
                        Name = "400",
                        AdditionPrice = 0
                    },
                    new WeightProduct
                    {
                        Id = 11,
                        Name = "1000",
                        AdditionPrice = 300
                    }
                },
                Types = new List<TypeProduct>
                {
                    new TypeProduct
                    {
                        Id = 10,
                        Name = "bulkosup",
                        Image = "image",
                        AdditionPrice = 300,
                    },
                    new TypeProduct
                    {
                        Id = 11,
                        Name = "kontainer",
                        Image = "image",
                        AdditionPrice = 0,
                    }
                }
            };

            var crtproduct = await service.CreateProduct(entity);

            var weight = entity.Weights.Where(x => x.Id == 10)
                                       .Select(x => x.Name)
                                       .FirstOrDefault();

            var type = entity.Types.Where(x => x.Id == 10)
                                   .Select(x => x.Name)
                                   .FirstOrDefault();

            var productPrice = entity.Price;

            await service.UpdateProduct(12, "Pizza", "Pizza", 3, 4, 525);
            await service.UpdateType(10, "Bulkosup", "Image", 250);
            await service.UpdateWeight(10, "450", 0);

            var updProduct = await service.GetProductById(12);
            var updWeight = _context.WeightProducts.Where(x => x.Id == 10)
                                                   .Select(x => x.Name)
                                                   .FirstOrDefault();

            var updType = _context.TypeProducts.Where(x => x.Id == 10)
                                               .Select(x => x.Name)
                                               .FirstOrDefault();

            Assert.NotNull(crtproduct);
            Assert.NotEqual(productPrice, updProduct.Price);
            Assert.NotEqual(updWeight, weight);
            Assert.NotEqual(updType, type);
        }

        [Fact]
        public async void DeleteProduct_DeleteProductWithTypesAndWeights_ReturnsRowDataAsync()
        {
            var service = new ProductService(_context);
            var entity = new Product
            {
                Id = 13,
                Title = "harcho",
                Description = "harcho",
                Category = 5,
                Rating = 5,
                Price = 400,
                Weights = new List<WeightProduct>
                {
                    new WeightProduct
                    {
                        Id = 14,
                        Name = "400",
                        AdditionPrice = 0
                    },
                    new WeightProduct
                    {
                        Id = 15,
                        Name = "1000",
                        AdditionPrice = 300
                    }
                },
                Types = new List<TypeProduct>
                {
                    new TypeProduct
                    {
                        Id = 14,
                        Name = "bulkosup",
                        Image = "image",
                        AdditionPrice = 300,
                    },
                    new TypeProduct
                    {
                        Id = 15,
                        Name = "kontainer",
                        Image = "image",
                        AdditionPrice = 0,
                    }
                }
            };
            var crtproduct = await service.CreateProduct(entity);
            var Delete = await service.DeleteProduct(crtproduct.Id);
            var found = await _context.Products.FirstOrDefaultAsync(x => 
                                                                x.Id == entity.Id);

            Assert.Null(found);
        }
    }
}