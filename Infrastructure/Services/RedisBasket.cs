using Core.DTOS.Basket;
using Core.DTOS.Email;
using Infrastructure.interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
   public class RedisBasket : IRedisService<CustomerBasket>
   {
       private readonly IDatabase _database;
       public RedisBasket(IConnectionMultiplexer redix)
       {
           _database = redix.GetDatabase();
       }

       public async Task<CustomerBasket?> GetAsync(string? item)
       {
           if (string.IsNullOrEmpty(item))
               return null;


           var data = await _database.StringGetAsync(item);

           return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data!);
       }


       public async Task<CustomerBasket?> UpdateAsync(CustomerBasket item)
       {
           var created = await _database.StringSetAsync(item.Id.ToString(),
               JsonSerializer.Serialize(item),
               // if the id not exist , it will created a new Code 
               TimeSpan.FromDays(30));

           if (!created)
               return null;

           return await GetAsync(item.Id.ToString()); // to get the object as basket object
       }
       public async Task<bool?> DeleteAsync(string? item)
       {
           if (string.IsNullOrEmpty(item))
               return null;

           return await _database.KeyDeleteAsync(item);
       }

       
   }
}
