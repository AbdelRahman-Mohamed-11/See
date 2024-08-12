using Core.DTOS.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.interfaces
{
    public interface ICacheService
    {
        T GetData<T>(string key);

        bool SetData<T>(string key,T value,DateTime exprirationDate);

        bool RemoveData(string key);

        CustomerBasket GetBasketByUserId(string userId);
    }
}
