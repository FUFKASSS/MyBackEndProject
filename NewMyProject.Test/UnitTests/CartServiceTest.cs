using Microsoft.EntityFrameworkCore;
using NewMyProject.Data;
using NewMyProject.Entities;
using NewMyProject.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NewMyProject.Test
{
    public class CartServiceTest : UnitTestBase
    {
        private  EfContext _context;
        private static readonly WeightProduct _weight1 = new WeightProduct
        {
            Id = 111,
            Name = "100",
            AdditionPrice = 0
        };
        private static readonly TypeProduct _type1 = new TypeProduct
        {
            Id = 111,
            Name = "BulkoSup",
            Image = "image",
            AdditionPrice = 200,
        };
        private static readonly Product _product1 = new Product
        {
            Id = 1,
            Title = "Soup",
            Description = "Soup",
            Category = 1,
            Rating = 1,
            Price = 100,
            Weights = new List<WeightProduct>
            {
                _weight1,
                new WeightProduct
                {
                    Id = 2,
                    Name = "300",
                    AdditionPrice = 200
                }
            },
            Types = new List<TypeProduct>
            {
                _type1,
                new TypeProduct
                {
                    Id = 2,
                    Name = "Kontainer",
                    Image = "image",
                    AdditionPrice = 0,
                }
            }
        };
        private static readonly Item _item1 = new Item
        {
            Id = 1,
            Count = 2,
            Type = _type1,
            Weight = _weight1,
            Product = _product1
        };
        private static readonly Order _order1 = new Order
        {
            Id = 1,
            ProfileId = 1,
            Status = 0,
            Items = new List<Item>
            {
                _item1
            }
        };
        private static readonly User _user1 = new User
        {
            Id = 1,
            Email = "test@mail.ru",
            Role = "User",
            PhoneNumber = 79856734731,
            UserName = "Test",
            Password = BCrypt.Net.BCrypt.HashPassword("asdasd"),
            Profile = _profile1,
        };
        private static readonly Profile _profile1 = new Profile
        {
            Id = 1,
            Email = "test@mail.ru",
            Name = "Test",
            Orders = new List<Order>
            {
                _order1,
            },
            PhoneNumber = 79856734731,
            UserInfoKey = 1,
        };

        public CartServiceTest()
        {
            _context = CreateInMemoryContext(x => x.AddRange(_product1, 
                                                             _order1, 
                                                             _user1, 
                                                             _profile1, 
                                                             _type1, 
                                                             _weight1));
        }

        [Fact]
        public async void AddToCart_AddItemToCart_ReturnsRowDataAsync()
        {
            var service = new CartService(_context);
            await service.AddToCart(_product1.Id, _weight1.Id, _type1.Id, 
                                    _user1.Id.ToString(), _profile1.Id.ToString());

            var found = await service.GetMyCart(_profile1.Id.ToString());
            Assert.NotNull(found);
        }

        [Fact]
        public async void DeleteFromCart_DeleteItemFromUserCart_ReturnsRowDataAsync()
        {
            var service = new CartService(_context);
            await service.DeleteFromCart(_product1.Id, _weight1.Id, _type1.Id, 
                                                        _profile1.Id.ToString());

            var found = await _context.Orders.FirstOrDefaultAsync(x => 
                                                        x.ProfileId == _profile1.Id);

            Assert.Null(found);
        }

        [Fact]
        public async void ChangeCartStatusFromAdmin_ChangeCartStatusToOrderStatus_ReturnsRowDataAsync()
        {
            // default_item_status == 0
            var service = new CartService(_context);
            var status =  _context.Orders.Where(x => x.ProfileId == 1)
                                         .Where(x => x.Status == (OrderStatus)0)
                                         .FirstOrDefault().Status;

            await service.ChangeCartStatusFromAdmin(1, 2);
            var updstatus = _context.Orders.Where(x => x.ProfileId == 1)
                                           .Where(x => x.Status == (OrderStatus)2)
                                           .FirstOrDefault().Status;

            Assert.NotEqual(status, updstatus);
        }

        [Fact]
        public async void DecrementAndIncrement_DecrementAndIncrementItemInCart_ReturnsRowData()
        {
            // default_item_count == 2
            var service = new CartService(_context);
            var count = _context.Items.FirstOrDefault(x => x.Count == _item1.Count).Count;
            await service.IncrementItemInCart(_product1.Id, _weight1.Id, _type1.Id, _profile1.Id.ToString());
            await service.IncrementItemInCart(_product1.Id, _weight1.Id, _type1.Id, _profile1.Id.ToString());
            await service.DecrementItemInCart(_product1.Id, _weight1.Id, _type1.Id, _profile1.Id.ToString());
            var updCount = _context.Items.FirstOrDefault(x => x.Count == 3).Count;
            Assert.NotEqual(count, updCount);
        }
    }
}
