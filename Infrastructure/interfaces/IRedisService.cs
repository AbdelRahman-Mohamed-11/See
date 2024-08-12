using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.interfaces
{
   public interface IRedisService<T> where T : class
   {
       Task<T?> GetAsync(string? item);

       Task<T?> UpdateAsync(T item);

       Task<bool?> DeleteAsync(string? item);
   }
}
