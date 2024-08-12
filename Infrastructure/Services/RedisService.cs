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
   public class RedisService : IRedisService<UserTokenVerificationEmail>
   {
       private readonly IDatabase _database;
       public RedisService(IConnectionMultiplexer redix)
       {
           _database = redix.GetDatabase();
       }

       public async Task<UserTokenVerificationEmail?> GetAsync(string? item)
       {
           if (string.IsNullOrEmpty(item))
               return null;


           var data = await _database.StringGetAsync(item);

           return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<UserTokenVerificationEmail>(data!);
       }


       public async Task<UserTokenVerificationEmail?> UpdateAsync(UserTokenVerificationEmail item)
       {
           var created = await _database.StringSetAsync(item.Email, JsonSerializer.Serialize(item),
               // if the id not exist , it will created a new Code 
               TimeSpan.FromMinutes(30));

           if (!created)
               return null;

           return await GetAsync(item.Email); // to get the object as basket object
       }
       public async Task<bool?> DeleteAsync(string? item)
       {
           if (string.IsNullOrEmpty(item))
               return null;

           return await _database.KeyDeleteAsync(item);
       }
   }
}
