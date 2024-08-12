using API.Errors;
using Application.Interfaces;
using Application.Services;
using Core.DTOS;
using Core.Entities.Order;
using Infrastructure.Identity;
using Infrastructure.interfaces;
using Infrastructure.Notifications;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlassesApp.Controllers
{
    
    public class OrdersController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrderInterface _orderService;
        private readonly INotificationService _notificationService;
        public OrdersController(UserManager<ApplicationUser> userManager
            , ApplicationDbContext applicationDb,
            INotificationService notificationService,
            IOrderInterface orderService) { 
            _userManager = userManager;
            _dbContext = applicationDb;
            _orderService = orderService;
            _notificationService = notificationService;

        }
        
        [HttpPost("create-order-basket")]
        [Authorize(Roles ="AppUser")]
        public async Task<ActionResult<Guid>> CreateOrder(
            OrderBasketDTO orderDTO)
        {
            var user = await _userManager
                .FindByIdAsync(User.FindFirst("ID")?.Value);

            Guid? orderId = await _orderService
                .CreateOrderBasketAsync(
                user.Id,
                orderDTO.firstName,
                orderDTO.lastName,
                 orderDTO.basketId,
                 orderDTO.IsAndroidDevice ?? null,
                 orderDTO.CityId,
                 orderDTO.street,
                orderDTO.newPhoneNumber, orderDTO.ExistingPhoneNumber,
                orderDTO.addressId
                ); ;

            if (orderId is null)
            {
                return BadRequest(new ApiResponse(400, "the order cannot created"));
            }

            if (orderDTO.IsAndroidDevice != null)
            {
                var notification = new NotificationModel
                {
                    DeviceId = user.DeviceId,
                    Body = "order has placed successfully !",
                    IsAndroidDevice = orderDTO.IsAndroidDevice.Value,
                    Title = "SEE - new Order"
                };

                await _notificationService.SendNotification(notification);

                await _dbContext.Notifications.AddAsync(
                      new Core.Entities.Notification
                      {
                          IsRead = false,
                          Message = notification.Body,
                          ApplicationUserId = user.Id,
                          Title = notification.Title,
                      }
                    );

                await _dbContext.SaveChangesAsync();
            }

            return Ok(new OrderIdDTO { OrderId = orderId.Value});

        }

        [HttpPost("order-for-product")]
        [Authorize(Roles = "AppUser")]

        public async Task<ActionResult<Guid>> CreateSingleProductOrderAsync(
            OrderDTOSingleProduct orderDto)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            var orderId = await 
                _orderService
                .CreateSingleProductOrderAsync(
                user.Id,
                orderDto.FirstName,
                orderDto.LastName,
                orderDto.productId,
                orderDto.colorName,
                orderDto.IsAndroidDevice == null ? null :orderDto.IsAndroidDevice.Value,
                orderDto.CityId, 
                orderDto.street,
                orderDto.newPhoneNumber, orderDto.ExistingPhoneNumber, orderDto.addressId,
                orderDto.userPrescriptionId);

            if (orderId is null)
                return BadRequest("the order cannot be done");

            if (orderDto.IsAndroidDevice != null)
            {
                var notification = new NotificationModel
                {
                    DeviceId = user.DeviceId,
                    Body = "order has placed successfully !",
                    IsAndroidDevice = orderDto.IsAndroidDevice.Value,
                    Title = "SEE - new Order"
                };

                await _notificationService.SendNotification(notification);

                await _dbContext.Notifications.AddAsync(
                      new Core.Entities.Notification
                      {
                          IsRead = false,
                          Message = notification.Body,
                          ApplicationUserId = user.Id,
                          Title = notification.Title,
                      }
                    );
                    
                await _dbContext.SaveChangesAsync();
            }
            return orderId;
        }

        [HttpPut("change-order-status")]
        [Authorize(Roles = "Manager,Admin")]

        public async Task<ActionResult<bool>> ChangeOrderStatus(Guid orderId , OrderStatus orderStatus)
        {
            var orderStatusUpdated = await _orderService.UpdateOrderStatusAsync(orderId, orderStatus);

            if (!orderStatusUpdated)
                return BadRequest("Cannot update order status");

            return Ok(orderStatusUpdated);
        }

        [HttpGet("get-user-orders")]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<Order>>> 
            GetOrdersForUser()
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var orders = await _orderService.GetOrders(baseUrl,user.Id);
            
            if (orders == null)
                return NotFound(new ApiResponse(404,"there are no orders"));
                
            return Ok(orders);
        }

        [HttpGet("get-orders-admin")]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<Order>>>
           GetOrdersForAdmin()
        {
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var orders = await _orderService.GetOrders(baseUrl,null);

            if (orders == null)
                return NotFound(new ApiResponse(404, "there are no orders"));

            return Ok(orders);
        }

        [HttpGet("manager-orders")]
        [Authorize(Roles = "Manager")]

        public async Task<ActionResult<IReadOnlyList<Order>?>> GetManagerOrders()
        {
            string id = User.FindFirst("ID")?.Value;

            if (id == null)
                throw new Exception("manager is NOT exist , Invalid ID");

            var orders = await _orderService.GetOrdersForManager(Guid.Parse(id));

            if(orders is null)
                return NotFound(new ApiResponse(404, "there are no orders"));

            return Ok(orders);
        }


        [HttpGet("track/{orderId}")]
        public async Task<ActionResult<OrderStatusWithDetailsDTO>> TrackOrder(Guid orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            if (order == null)
                return NotFound();

            var trackingInfo = new OrderStatusWithDetailsDTO
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                OrderStartDate = order.OrderDate,
                EstimatedDeliveryTime = _orderService.CalculateEstimatedDeliveryTime(order),
                Subtotal = order.SubTotal,
                Address = order.ShipToAddress,
                OrderItems = order.OrderItems
            };

            return Ok(trackingInfo);
        }

       

    }
}
