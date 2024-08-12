using Core.Entities.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICityService
    {
        public Task<Guid> CreateCity(string city);

        public Task<bool> UpdateCity(Guid cityId, string cityName);

        public Task<bool> DeleteCity(Guid cityId);

        public Task<List<City>> GetCities();

    }
}
