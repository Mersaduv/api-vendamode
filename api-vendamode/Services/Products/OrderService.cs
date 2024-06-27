using api_vendace.Data;
using api_vendace.Entities;
using api_vendace.Entities.Users;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.AuthDto;
using api_vendace.Models.Query;
using api_vendace.Utility;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos.ProductDto.Order;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace api_vendamode.Services.Products;

public class OrderService : IOrderServices
{
    private readonly ApplicationDbContext _context;
    private readonly IUserServices _userServices;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IUnitOfWork _unitOfWork;


    public OrderService(ApplicationDbContext context,
        IUserServices authService,
        ByteFileUtility byteFileUtility,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _userServices = authService;
        _byteFileUtility = byteFileUtility;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<Guid>> PlaceOrder(Guid id)
    {
        var userId = _userServices.GetUserId();
        var orderContext = new Order();
        orderContext = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

        if (orderContext == null)
        {
            return new ServiceResponse<Guid>
            {
                Message = "سفارشی با این شناسه وجود ندارد"
            };
        }
        orderContext.Status = 2; //2
        orderContext.Delivered = false; //false
        orderContext.Paid = true;
        orderContext.DateOfPayment = DateTime.UtcNow;


        _context.Orders.Update(orderContext);

        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<Guid>
        {
            Data = orderContext.Id,
            Message = "سفارش شما ثبت شد"
        };
    }

    public async Task<ServiceResponse<Guid>> CreateOrder(OrderCreateDTO orderCreate)
    {
        var userId = _userServices.GetUserId();
        var orderContext = new Order();
        var orderDto = new OrderDTO();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNum = GenerateOrderCode(null),
            Status = orderCreate.Status,
            UserID = userId,
            AddressId = orderCreate.Address,
            Cart = orderCreate.Cart,
            CanceledId = orderCreate.CanceledId == Guid.Empty ? null : orderCreate.CanceledId,
            TotalItems = orderCreate.TotalItems,
            TotalDiscount = orderCreate.TotalDiscount,
            TotalPrice = orderCreate.TotalPrice,
            OrgPrice = orderCreate.OrgPrice,
            PaymentMethod = orderCreate.PaymentMethod,
            Delivered = false,
            Paid = false,
            PurchaseInvoice = orderCreate.Thumbnail != null ? _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Order>>([orderCreate.Thumbnail], nameof(Order)).First() : null,
            DateOfPayment = DateTime.UtcNow,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
        };

        try
        {
            // Detach existing entities that may be tracked
            var existingOrder = await _context.Orders.FindAsync(order.Id);
            if (existingOrder != null)
            {
                _context.Entry(existingOrder).State = EntityState.Detached;
            }

            // Ensure unique primary keys for ObjectValue and other entities
            foreach (var cartItem in order.Cart)
            {
                if (cartItem.Features != null)
                {
                    cartItem.Features.Id = Guid.NewGuid();
                    foreach (var value in cartItem.Features.Value)
                    {
                        value.Id = Guid.NewGuid();
                    }
                }
            }

            await _context.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResponse<Guid>
            {
                Data = order.Id,
                Message = "سفارش شما ثبت شد"
            };
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex);
            if (ex.InnerException != null)
            {
                Console.WriteLine(ex.InnerException.Message);
            }
            return new ServiceResponse<Guid>
            {
                Data = Guid.Empty,
                Message = $"An error occurred: {ex.Message}"
            };
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex);
            return new ServiceResponse<Guid>
            {
                Data = Guid.Empty,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }



    private string GenerateOrderCode(string? k)
    {
        string prefix = k is not null ? "Kl" : "Ks";
        long lastCodeNumber = GetLastOrderCodeNumber();
        long newCodeNumber = lastCodeNumber + 1;
        string formattedCodeNumber = newCodeNumber.ToString("D9");
        string newCode = prefix + formattedCodeNumber;
        return newCode;
    }

    private long GetLastOrderCodeNumber()
    {
        var lastProduct = _context.Orders
            .OrderByDescending(p => p.OrderNum)
            .FirstOrDefault();

        if (lastProduct == null)
        {
            return 0;
        }

        string orderNum = lastProduct.OrderNum;
        string numericPart = new string(orderNum.Where(char.IsDigit).ToArray());

        if (long.TryParse(numericPart, out long lastCodeNumber))
        {
            return lastCodeNumber;
        }
        else
        {
            throw new FormatException($"Invalid order number format: {orderNum}");
        }
    }


    public async Task<ServiceResponse<OrderResult>> GetOrders(RequestQuery requestQuery)
    {
        var userId = _userServices.GetUserId();
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize ?? 15;
        var skipCount = (pageNumber - 1) * pageSize;
        var orders = await _context.Orders
            .Where(r => r.UserID == userId)
            .OrderByDescending(r => r.Created)
            .Skip(skipCount)
            .Take(pageSize)
            .Include(c => c.Cart)
            .Include(u => u.User)
            .Include(a => a.Address)
            .Include(c => c.Canceled)
            .Include(o => o.PurchaseInvoice)
            .ToListAsync();

        var orderList = orders.Select(r => new OrderDTO
        {
            Id = r.Id,
            OrderNum = r.OrderNum,
            Status = r.Status,
            UserID = r.UserID,
            User = new UserDTO
            {
                Id = r.UserID,
                MobileNumber = r.Address.MobileNumber,
            },
            Address = r.Address,
            Cart = r.Cart,
            Canceled = r.Canceled ?? null,
            TotalItems = r.TotalItems,
            TotalPrice = r.TotalPrice,
            TotalDiscount = r.TotalDiscount,
            PaymentMethod = r.PaymentMethod,
            Delivered = r.Delivered,
            Paid = r.Paid,
            // PurchaseInvoice = _byteFileUtility.GetEncryptedFileActionUrl
            //                 ([new EntityImageDto
            //                 {
            //                     Id = r.PurchaseInvoice!.Id,
            //                     ImageUrl = r.PurchaseInvoice.ImageUrl!,
            //                     Placeholder = r.PurchaseInvoice.Placeholder!
            //                 }], nameof(Order)).First(),
            DateOfPayment = r.DateOfPayment,
            Created = r.Created,
            LastUpdated = r.LastUpdated
        }).ToList();

        var totalOrders = await _context.Orders.CountAsync(r => r.UserID == userId);

        var pagination = new Pagination<OrderDTO>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber + 1,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 0,
            HasNextPage = (skipCount + pageSize) < totalOrders,
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((decimal)totalOrders / pageSize),
            TotalCount = totalOrders,
            Data = orderList
        };

        var result = new OrderResult
        {
            OrdersLength = totalOrders,
            Pagination = pagination
        };

        return new ServiceResponse<OrderResult>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<bool>> UpdateOrder(OrderUpsertDTO orderUpdate)
    {
        var userId = _userServices.GetUserId();
        var order = await _context.Orders
            .Include(o => o.PurchaseInvoice)
            .FirstOrDefaultAsync(o => o.Id == orderUpdate.OrderId && o.UserID == userId);

        if (order == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "سفارشی با این شناسه وجود ندارد"
            };
        }

        order.Status = orderUpdate.Status;
        order.Delivered = orderUpdate.Delivered;
        order.Paid = orderUpdate.Paid;
        order.LastUpdated = DateTime.UtcNow;
        order.Updated = DateTime.UtcNow;

        _context.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "سفارش شما با موفقیت تغییر وضعیت کرد"
        };
    }

    public async Task<ServiceResponse<bool>> UpdateOrderStatus(CancelOrderUpdateStatus orderUpdate)
    {
        var userId = _userServices.GetUserId();
        var order = await _context.Orders
            .Include(o => o.Cart)
            .FirstOrDefaultAsync(o => o.Id == orderUpdate.OrderId && o.UserID == userId);

        if (order == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "سفارشی با این شناسه وجود ندارد"
            };
        }

        if (order.Cart == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "آیتمی در این سفارش وجود ندارد"
            };
        }

        if (orderUpdate.ItemID == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "سفارشی با این شناسه وجود ندارد"
            };
        }

        var canceledItems = order.Cart.Where(c => orderUpdate.ItemID.Contains(c.ItemID)).ToList();
        var remainingItems = order.Cart.Where(c => !orderUpdate.ItemID.Contains(c.ItemID)).ToList();

        // Remove items from the original order
        foreach (var item in canceledItems)
        {
            _context.Entry(item).State = EntityState.Deleted;
        }
        await _unitOfWork.SaveChangesAsync();

        // Create a new order for canceled items if there are any
        if (canceledItems.Count > 0)
        {
            var newOrderId = Guid.NewGuid();
            var newOrder = new Order
            {
                Id = newOrderId,
                OrderNum = GenerateOrderCode("l"),
                Status = orderUpdate.Status,
                UserID = order.UserID,
                User = order.User,
                AddressId = order.AddressId,
                Address = order.Address,
                Cart = new List<Cart>(),
                CanceledId = orderUpdate.CanceledId,
                Canceled = order.Canceled,
                TotalItems = order.TotalItems,
                TotalPrice = order.TotalPrice,
                OrgPrice = order.OrgPrice,
                TotalDiscount = order.TotalDiscount,
                PaymentMethod = order.PaymentMethod,
                Delivered = order.Delivered,
                Paid = order.Paid,
                PurchaseInvoice = order.PurchaseInvoice,
                DateOfPayment = order.DateOfPayment,
                Updated = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                Created = order.Created
            };

            foreach (var item in canceledItems)
            {
                item.OrderId = newOrderId;
                newOrder.Cart.Add(item);
            }
            await _context.Orders.AddAsync(newOrder);
        }

        // Update the original order to exclude canceled items
        if (remainingItems.Count > 0)
        {
            order.Cart = remainingItems;
            order.Updated = DateTime.UtcNow;
            _context.Orders.Update(order);
        }
        else
        {
            _context.Orders.Remove(order);
        }

        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "سفارش شما با موفقیت تغییر وضعیت کرد"
        };
    }


    public async Task<ServiceResponse<bool>> DeleteOrder(Guid orderId)
    {
        var userId = _userServices.GetUserId();
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserID == userId);

        if (order == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "سفارشی با این شناسه وجود ندارد"
            };
        }

        _context.Orders.Remove(order);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "سفارش با موفقیت حذف شد"
        };
    }

    // public async Task<ServiceResponse<bool>> UpdateOrderReturnedStatus(OrderReturnedDTO orderUpdate)
    // {
    //     var userId = _userServices.GetUserId();
    //     var newOrderId = Guid.NewGuid();

    //     var order = await _context.Orders
    //         .Include(o => o.Cart).ThenInclude(o => o.Features)
    //         .FirstOrDefaultAsync(o => o.Id == orderUpdate.OrderId && o.UserID == userId);

    //     if (order == null)
    //     {
    //         return new ServiceResponse<bool>
    //         {
    //             Data = false,
    //             Message = "سفارشی با این شناسه وجود ندارد"
    //         };
    //     }

    //     if (order.Cart == null)
    //     {
    //         return new ServiceResponse<bool>
    //         {
    //             Data = false,
    //             Message = "آیتمی در این سفارش وجود ندارد"
    //         };
    //     }

    //     Order orderHelper = new Order
    //     {
    //         Id = order.Id,
    //         OrderNum = order.OrderNum,
    //         Status = order.Status,
    //         UserID = order.UserID,
    //         User = order.User,
    //         AddressId = order.AddressId,
    //         Address = order.Address,
    //         Cart = new List<Cart>(order.Cart), // Create a new list with the same items
    //         Returned = order.Returned,
    //         ReturnedId = order.ReturnedId,
    //         TotalItems = order.TotalItems,
    //         TotalPrice = order.TotalPrice,
    //         OrgPrice = order.OrgPrice,
    //         TotalDiscount = order.TotalDiscount,
    //         PaymentMethod = order.PaymentMethod,
    //         Delivered = order.Delivered,
    //         Paid = order.Paid,
    //         PurchaseInvoice = order.PurchaseInvoice,
    //         DateOfPayment = order.DateOfPayment,
    //         Updated = order.Updated,
    //         LastUpdated = order.LastUpdated,
    //         Created = order.Created
    //     };

    //     var filteredCart = new List<Cart>();
    //     var remainingCart = new List<Cart>();

    //     foreach (var cartItem in order.Cart)
    //     {
    //         var updateItem = orderUpdate.Items.FirstOrDefault(item => item.Id == cartItem.ItemID);
    //         if (updateItem != null)
    //         {
    //             var remainingQuantity = cartItem.Quantity - updateItem.Quantity;
    //             if (updateItem.Quantity > 0)
    //             {
    //                 filteredCart.Add(new Cart
    //                 {
    //                     Id = Guid.NewGuid(),
    //                     OrderId = newOrderId,
    //                     ItemID = cartItem.ItemID,
    //                     ProductID = cartItem.ProductID,
    //                     Name = cartItem.Name,
    //                     Slug = cartItem.Slug,
    //                     Price = cartItem.Price,
    //                     Discount = cartItem.Discount,
    //                     InStock = cartItem.InStock,
    //                     Sold = cartItem.Sold,
    //                     Color = cartItem.Color,
    //                     Size = cartItem.Size,
    //                     Features = cartItem.Features,
    //                     Img = cartItem.Img,
    //                     Quantity = updateItem.Quantity
    //                 });
    //             }
    //             if (remainingQuantity > 0)
    //             {
    //                 remainingCart.Add(new Cart
    //                 {
    //                     Id = cartItem.Id,
    //                     OrderId = cartItem.OrderId,
    //                     ItemID = cartItem.ItemID,
    //                     ProductID = cartItem.ProductID,
    //                     Name = cartItem.Name,
    //                     Slug = cartItem.Slug,
    //                     Price = cartItem.Price,
    //                     Discount = cartItem.Discount,
    //                     InStock = cartItem.InStock,
    //                     Sold = cartItem.Sold,
    //                     Color = cartItem.Color,
    //                     Size = cartItem.Size,
    //                     Features = cartItem.Features,
    //                     Img = cartItem.Img,
    //                     Quantity = remainingQuantity
    //                 });
    //             }
    //         }
    //     }
    //     if (order.Cart.Count > 0)
    //     {
    //         _context.Cart.RemoveRange(order.Cart);
    //         await _unitOfWork.SaveChangesAsync();
    //     }
    //     if (filteredCart.Count > 0)
    //     {
    //         var newOrder = new Order
    //         {
    //             Id = newOrderId,
    //             OrderNum = GenerateOrderCode("l"),
    //             Status = 4,
    //             UserID = order.UserID,
    //             User = order.User,
    //             AddressId = order.AddressId,
    //             Address = order.Address,
    //             Cart = filteredCart,
    //             Returned = order.Returned,
    //             ReturnedId = orderUpdate.ReturnedId,
    //             TotalItems = order.TotalItems,
    //             TotalPrice = order.TotalPrice,
    //             OrgPrice = order.OrgPrice,
    //             TotalDiscount = order.TotalDiscount,
    //             PaymentMethod = order.PaymentMethod,
    //             Delivered = order.Delivered,
    //             Paid = order.Paid,
    //             PurchaseInvoice = order.PurchaseInvoice,
    //             DateOfPayment = order.DateOfPayment,
    //             Updated = DateTime.UtcNow,
    //             LastUpdated = DateTime.UtcNow,
    //             Created = order.Created
    //         };
    //         await _context.Orders.AddAsync(newOrder);
    //         await _unitOfWork.SaveChangesAsync();
    //     }

    //     if (remainingCart.Count > 0)
    //     {
    //         var remainingCartIds = new List<Guid>();
    //         foreach (var item in remainingCart)
    //         {
    //             remainingCartIds.Add(item.Id);
    //         }
    //         _context.Entry(order).State = EntityState.Detached;
    //         foreach (var cartItem in order.Cart)
    //         {
    //             _context.Entry(cartItem).State = EntityState.Detached;
    //         }
    //         orderHelper.Cart.RemoveAll(cartItem => remainingCartIds.Contains(cartItem.Id));
    //         orderHelper.Cart.AddRange(remainingCart);
    //         orderHelper.Updated = DateTime.UtcNow;
    //         _context.Orders.Update(orderHelper);
    //         await _unitOfWork.SaveChangesAsync();
    //     }
    //     else
    //     {
    //         // _context.Orders.Remove(order);
    //         await _unitOfWork.SaveChangesAsync();
    //     }

    //     return new ServiceResponse<bool>
    //     {
    //         Data = true,
    //         Message = "سفارش شما با موفقیت تغییر وضعیت کرد"
    //     };
    // }


    public async Task<ServiceResponse<bool>> UpdateOrderReturnedStatus(OrderReturnedDTO orderUpdate)
    {
        var userId = _userServices.GetUserId();
        var newOrderId = Guid.NewGuid();
        var order = await _context.Orders
            .Include(o => o.Cart)
            .FirstOrDefaultAsync(o => o.Id == orderUpdate.OrderId && o.UserID == userId);

        if (order == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "سفارشی با این شناسه وجود ندارد"
            };
        }

        if (order.Cart == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "آیتمی در این سفارش وجود ندارد"
            };
        }

        Order orderHelper = new Order
        {
            Id = order.Id,
            OrderNum = order.OrderNum,
            Status = order.Status,
            UserID = order.UserID,
            User = order.User,
            AddressId = order.AddressId,
            Address = order.Address,
            Cart = new List<Cart>(order.Cart), // Create a new list with the same items
            Returned = order.Returned,
            ReturnedId = order.ReturnedId,
            TotalItems = order.TotalItems,
            TotalPrice = order.TotalPrice,
            OrgPrice = order.OrgPrice,
            TotalDiscount = order.TotalDiscount,
            PaymentMethod = order.PaymentMethod,
            Delivered = order.Delivered,
            Paid = order.Paid,
            PurchaseInvoice = order.PurchaseInvoice,
            DateOfPayment = order.DateOfPayment,
            Updated = order.Updated,
            LastUpdated = order.LastUpdated,
            Created = order.Created
        };

        var filteredCart = new List<Cart>();
        var remainingCart = new List<Cart>();

        foreach (var cartItem in order.Cart)
        {
            var updateItem = orderUpdate.Items.FirstOrDefault(item => item.Id == cartItem.ItemID);
            if (updateItem != null)
            {
                var remainingQuantity = cartItem.Quantity - updateItem.Quantity;
                if (updateItem.Quantity > 0)
                {
                    filteredCart.Add(new Cart
                    {
                        Id = Guid.NewGuid(),
                        OrderId = newOrderId,
                        ItemID = Nanoid.Generate(),
                        ProductID = cartItem.ProductID,
                        Name = cartItem.Name,
                        Slug = cartItem.Slug,
                        Price = cartItem.Price,
                        Discount = cartItem.Discount,
                        InStock = cartItem.InStock,
                        Sold = cartItem.Sold,
                        Color = cartItem.Color,
                        Size = cartItem.Size,
                        Features = cartItem.Features,
                        Img = cartItem.Img,
                        Quantity = updateItem.Quantity
                    });
                }
                if (remainingQuantity > 0)
                {
                    remainingCart.Add(new Cart
                    {
                        Id = cartItem.Id,
                        OrderId = cartItem.OrderId,
                        ItemID = cartItem.ItemID,
                        ProductID = cartItem.ProductID,
                        Name = cartItem.Name,
                        Slug = cartItem.Slug,
                        Price = cartItem.Price,
                        Discount = cartItem.Discount,
                        InStock = cartItem.InStock,
                        Sold = cartItem.Sold,
                        Color = cartItem.Color,
                        Size = cartItem.Size,
                        Features = cartItem.Features,
                        Img = cartItem.Img,
                        Quantity = remainingQuantity
                    });
                }
            }
        }


        if (order.Cart.Count > 0)
        {
            _context.Cart.RemoveRange(order.Cart);
            await _unitOfWork.SaveChangesAsync();
        }

        if (filteredCart.Count > 0)
        {
            var newOrder = new Order
            {
                Id = newOrderId,
                OrderNum = GenerateOrderCode("l"),
                Status = 4,
                UserID = order.UserID,
                User = order.User,
                AddressId = order.AddressId,
                Address = order.Address,
                Cart = filteredCart,
                Returned = order.Returned,
                ReturnedId = orderUpdate.ReturnedId,
                TotalItems = order.TotalItems,
                TotalPrice = order.TotalPrice,
                OrgPrice = order.OrgPrice,
                TotalDiscount = order.TotalDiscount,
                PaymentMethod = order.PaymentMethod,
                Delivered = order.Delivered,
                Paid = order.Paid,
                PurchaseInvoice = order.PurchaseInvoice,
                DateOfPayment = order.DateOfPayment,
                Updated = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                Created = order.Created
            };
            await _context.Cart.AddRangeAsync(newOrder.Cart);
            await _context.Orders.AddAsync(newOrder);
            await _unitOfWork.SaveChangesAsync();
        }

        if (remainingCart.Count > 0)
        {
            // Detach the original order to avoid tracking issues
            _context.Entry(order).State = EntityState.Detached;
            var remainingCartIds = new List<Guid>();
            foreach (var item in remainingCart)
            {
                remainingCartIds.Add(item.Id);
            }
            orderHelper.Cart.RemoveAll(cartItem => remainingCartIds.Contains(cartItem.Id));
            orderHelper.Cart.AddRange(remainingCart);
            await _context.Cart.AddRangeAsync(orderHelper.Cart);
            await _unitOfWork.SaveChangesAsync();
            
            _context.Orders.Attach(orderHelper);
            _context.Entry(orderHelper).State = EntityState.Modified;
            await _unitOfWork.SaveChangesAsync();
        }

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "سفارش شما با موفقیت تغییر وضعیت کرد"
        };
    }



    // public async Task<ServiceResponse<bool>> UpdateOrderReturnedStatus(OrderReturnedDTO orderUpdate)
    // {
    //     var userId = _userServices.GetUserId();
    //     var newOrderId = Guid.NewGuid();
    //     var order = await _context.Orders
    //         .Include(o => o.Cart)
    //         .FirstOrDefaultAsync(o => o.Id == orderUpdate.OrderId && o.UserID == userId);

    //     if (order == null)
    //     {
    //         return new ServiceResponse<bool>
    //         {
    //             Data = false,
    //             Message = "سفارشی با این شناسه وجود ندارد"
    //         };
    //     }

    //     if (order.Cart == null)
    //     {
    //         return new ServiceResponse<bool>
    //         {
    //             Data = false,
    //             Message = "آیتمی در این سفارش وجود ندارد"
    //         };
    //     }

    //     var filteredCart = new List<Cart>();
    //     var remainingCart = new List<Cart>();

    //     foreach (var cartItem in order.Cart)
    //     {
    //         var updateItem = orderUpdate.Items.FirstOrDefault(item => item.Id == cartItem.ItemID);
    //         if (updateItem != null)
    //         {
    //             var remainingQuantity = cartItem.Quantity - updateItem.Quantity;
    //             if (updateItem.Quantity > 0)
    //             {
    //                 filteredCart.Add(new Cart
    //                 {
    //                     Id = Guid.NewGuid(),
    //                     OrderId = newOrderId,
    //                     ItemID = Nanoid.Generate(),
    //                     ProductID = cartItem.ProductID,
    //                     Name = cartItem.Name,
    //                     Slug = cartItem.Slug,
    //                     Price = cartItem.Price,
    //                     Discount = cartItem.Discount,
    //                     InStock = cartItem.InStock,
    //                     Sold = cartItem.Sold,
    //                     Color = cartItem.Color,
    //                     Size = cartItem.Size,
    //                     Features = cartItem.Features,
    //                     Img = cartItem.Img,
    //                     Quantity = updateItem.Quantity
    //                 });
    //             }
    //             if (remainingQuantity > 0)
    //             {
    //                 remainingCart.Add(new Cart
    //                 {
    //                     Id = cartItem.Id,
    //                     OrderId = cartItem.OrderId,
    //                     ItemID = cartItem.ItemID,
    //                     ProductID = cartItem.ProductID,
    //                     Name = cartItem.Name,
    //                     Slug = cartItem.Slug,
    //                     Price = cartItem.Price,
    //                     Discount = cartItem.Discount,
    //                     InStock = cartItem.InStock,
    //                     Sold = cartItem.Sold,
    //                     Color = cartItem.Color,
    //                     Size = cartItem.Size,
    //                     Features = cartItem.Features,
    //                     Img = cartItem.Img,
    //                     Quantity = remainingQuantity
    //                 });
    //             }
    //         }
    //     }


    //     if (order.Cart.Count > 0)
    //     {
    //         _context.Cart.RemoveRange(order.Cart);
    //         await _unitOfWork.SaveChangesAsync();
    //     }

    //     if (filteredCart.Count > 0)
    //     {
    //         var newOrder = new Order
    //         {
    //             Id = newOrderId,
    //             OrderNum = GenerateOrderCode("l"),
    //             Status = 4,
    //             UserID = order.UserID,
    //             User = order.User,
    //             AddressId = order.AddressId,
    //             Address = order.Address,
    //             Cart = filteredCart,
    //             Returned = order.Returned,
    //             ReturnedId = orderUpdate.ReturnedId,
    //             TotalItems = order.TotalItems,
    //             TotalPrice = order.TotalPrice,
    //             OrgPrice = order.OrgPrice,
    //             TotalDiscount = order.TotalDiscount,
    //             PaymentMethod = order.PaymentMethod,
    //             Delivered = order.Delivered,
    //             Paid = order.Paid,
    //             PurchaseInvoice = order.PurchaseInvoice,
    //             DateOfPayment = order.DateOfPayment,
    //             Updated = DateTime.UtcNow,
    //             LastUpdated = DateTime.UtcNow,
    //             Created = order.Created
    //         };
    //         await _context.Orders.AddAsync(newOrder);
    //         await _unitOfWork.SaveChangesAsync();
    //     }

    //     if (remainingCart.Count > 0)
    //     {
    //         // Detach the original order to avoid tracking issues
    //         _context.Entry(order).State = EntityState.Detached;

    //         // Attach the order without tracking
    //         var orderHelper = new Order
    //         {
    //             Id = order.Id,
    //             OrderNum = order.OrderNum,
    //             Status = order.Status,
    //             UserID = order.UserID,
    //             User = order.User,
    //             AddressId = order.AddressId,
    //             Address = order.Address,
    //             Cart = remainingCart,
    //             Returned = order.Returned,
    //             ReturnedId = order.ReturnedId,
    //             TotalItems = order.TotalItems,
    //             TotalPrice = order.TotalPrice,
    //             OrgPrice = order.OrgPrice,
    //             TotalDiscount = order.TotalDiscount,
    //             PaymentMethod = order.PaymentMethod,
    //             Delivered = order.Delivered,
    //             Paid = order.Paid,
    //             PurchaseInvoice = order.PurchaseInvoice,
    //             DateOfPayment = order.DateOfPayment,
    //             Updated = DateTime.UtcNow,
    //             LastUpdated = DateTime.UtcNow,
    //             Created = order.Created
    //         };

    //         _context.Orders.Attach(orderHelper);
    //         _context.Entry(orderHelper).State = EntityState.Modified;
    //         await _unitOfWork.SaveChangesAsync();
    //     }

    //     return new ServiceResponse<bool>
    //     {
    //         Data = true,
    //         Message = "سفارش شما با موفقیت تغییر وضعیت کرد"
    //     };
    // }



}