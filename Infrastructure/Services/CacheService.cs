using Core.DTOS.Basket;
using Infrastructure.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private ObjectCache _memoryCache = MemoryCache.Default;
        public T GetData<T>(string key)
        {
            T item = (T) _memoryCache.Get(key);

            return item;
        }

        public bool RemoveData(string key)
        {
            if (String.IsNullOrEmpty(key))
                return false;

            _memoryCache.Remove(key);

            return true;
        }

        // you can call it to update the caching
        public bool SetData<T>(string key, T value, DateTime exprirationDate)
        {
            if (String.IsNullOrEmpty(key) || value == null)
                     return false;

            _memoryCache.Set(key, value, exprirationDate);

            return true;
        }

        public CustomerBasket GetBasketByUserId(string userId)
        {
            // Iterate through all baskets in the cache
            foreach (var basket in _memoryCache)
            {
                // Check if the basket is of type CustomerBasket
                // and if its userId matches the provided userId
                if (basket is KeyValuePair<string, object> kvp && kvp.Value 
                    is CustomerBasket customerBasket && 
                    customerBasket.UserId.ToString() == userId)
                {
                    return customerBasket;
                }
            }

            //No basket is found with the userId
            return null;
        }
    }
}
