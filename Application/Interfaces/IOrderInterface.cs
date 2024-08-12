using Core.DTOS;
using Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderInterface
    {
        Task<Guid?> CreateOrderBasketAsync(
            Guid userId,
            string firstName,
            string lastName,
            Guid basketId,
            bool? isAndroidDevice,
            Guid? cityId,
            string? street, string?
            newPhoneNumber, Guid? existPhoneNumberId,
            Guid? addressId);

        Task<Guid?> CreateSingleProductOrderAsync(
             Guid userId,
             string firstName,
             string lastName,
             Guid productId,
             string colorName,
             bool? isAndroidDevice,
             Guid? cityId,
             string? street,
             string? newPhoneNumber,
             Guid? existPhoneNumberId,
             Guid? addressId,
             Guid? userPrescrptionId);

        Task<List<Order>?> GetOrders(string baseUrl, Guid? userId);

        Task<Order?> GetOrderById(Guid id);

        Task<List<Order>?> GetOrdersForManager(Guid managerId);

        Task<IReadOnlyList<Order>?> GetOrdersAsync();


        public DateTime CalculateEstimatedDeliveryTime(Order order);


        public Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
    }


}
