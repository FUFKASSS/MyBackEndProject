using Microsoft.EntityFrameworkCore;
using NewMyProject.Data;
using NewMyProject.DTO.responseDto;
using NewMyProject.Entities;
using NewMyProject.Services.Interfaces;

namespace NewMyProject.Services
{
    public class CartService : ICartService
    {
        private readonly EfContext _context;

        public CartService(EfContext context)
        {
            _context = context;
        }

        public async Task<ResponseStatus> AddToCart(int productId, int weightId, 
                                                    int typeId, string userId, 
                                                    string profileId, int status = 0)
        {
            try
            {
                int count = 1;

                var product = _context.Products.FirstOrDefault(c => c.Id == productId);
                var weight = _context.WeightProducts.FirstOrDefault(c => c.Id == weightId);
                var type = _context.TypeProducts.FirstOrDefault(c => c.Id == typeId);

                var item = _context.Items.Where(c => c.Type.Id == typeId)
                                         .Where(c => c.Weight.Id == weightId)
                                         .Where(c => c.Product.Id == productId)
                                         .FirstOrDefault();
                if (item == null)
                {
                    var newOrder = new Order
                    {
                        Status = (OrderStatus)status,
                        ProfileId = int.Parse(profileId),
                        Items = new List<Item>
                        {
                            new Item
                            {
                                Count = count,
                                Type = type,
                                Weight = weight,
                                Product = product,
                            },
                        },
                    };

                    await _context.Orders.AddAsync(newOrder);
                    await _context.SaveChangesAsync();

                    return new ResponseStatus
                    {
                        Status = "Success. Created."
                    };
                }

                else
                {
                    item.Count++;
                    await _context.SaveChangesAsync();

                    return new ResponseStatus
                    {
                        Status = "Success. Plusses."
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

        public async Task<List<Order>> GetMyCart(string profileId)
        {
            var OrderItems = (from orders in _context.Orders
                              from items in orders.Items
                              let products = items.Product
                              let weights = items.Weight
                              let types = items.Type
                              select new{items, products, weights, types, orders});

            OrderItems.ToList();
            return _context.Orders.Where(x => 
                                         x.ProfileId.ToString() == profileId).ToList();
        }

        public async Task<ResponseStatus> IncrementItemInCart(int productId, int weightId,
                                                              int typeId, string profileId)
        {
            try
            {
                var item = _context.Items.Where(c => c.Type.Id == typeId)
                                         .Where(c => c.Weight.Id == weightId)
                                         .Where(c => c.Product.Id == productId)
                                         .FirstOrDefault();

                if (item.Count < 15)
                {
                    item.Count++;
                    await _context.SaveChangesAsync();

                    return new ResponseStatus
                    {
                        Status = "Success. Plusses."
                    };
                }
                else
                {
                    return new ResponseStatus
                    {
                        Status = "Succes. Plusses limit"
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

        public async Task<ResponseStatus> DecrementItemInCart(int productId, int weightId,
                                                              int typeId, string profileId)
        {
            try
            {
                var item = _context.Items.Where(c => c.Type.Id == typeId)
                                         .Where(c => c.Weight.Id == weightId)
                                         .Where(c => c.Product.Id == productId)
                                         .FirstOrDefault();
                if (item.Count > 0)
                {
                    item.Count--;
                    await _context.SaveChangesAsync();

                    return new ResponseStatus
                    {
                        Status = "Success. Minueses."
                    };
                }
                else
                {
                    return new ResponseStatus
                    {
                        Status = "Succes. Minueses limit"
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

        public async Task<ResponseStatus> DeleteFromCart(int productId, int weightId,
                                                        int typeId, string profileId)
        {
            try
            {
               await DeleteOrder(profileId);
               await DeleteItem(productId, weightId, typeId);

               return new ResponseStatus
               {
                   Status = "Success. Deleted."
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

        private async Task<ResponseStatus> DeleteOrder(string profileId)
        {
            try
            {
                var order = _context.Orders
                                     .Where(x => x.ProfileId == int.Parse(profileId))
                                     .FirstOrDefaultAsync();

                _context.Orders.Remove(await order);
                await _context.SaveChangesAsync();

                return new ResponseStatus
                {
                    Status = "Success. Deleted."
                };
            }
            catch(Exception ex)
            {
                return new ResponseStatus
                {
                    Status = ex.Message
                };
            }
        }

        private async Task<ResponseStatus> DeleteItem(int productId, 
                                                      int weightId, int typeId)
        {
            try
            {
                var items = _context.Items.Where(c => c.Type.Id == typeId)
                                          .Where(c => c.Weight.Id == weightId)
                                          .Where(c => c.Product.Id == productId)
                                          .FirstOrDefaultAsync();

                _context.Items.Remove(await items);
                await _context.SaveChangesAsync();

                return new ResponseStatus
                {
                    Status = "Success. Deleted."
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

        public async Task<ResponseStatus> ChangeCartStatusFromAdmin(int id,
                                                                     int status)
        {
            try
            {
                var Items = (from orders in _context.Orders
                             from items in orders.Items
                             let products = items.Product
                             let weights = items.Weight
                             let types = items.Type
                             select new { products, weights, types, 
                                          orders, items });
                Items.ToList();

                var order = await _context.Orders.Where(x => x.Id == id)
                                                 .FirstOrDefaultAsync();

                order.Status = (OrderStatus)status;
                await _context.SaveChangesAsync();

                return new ResponseStatus
                {
                    Status = "Success. Status Changed."
                };
            }
            catch(Exception ex)
            {
                return new ResponseStatus
                {
                    Status = ex.Message
                };
            }
        }
    }
}
