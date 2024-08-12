using Application.Interfaces;
using Core.Entities.Delivery;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CityService : ICityService
    {
        private readonly ApplicationDbContext _db;

        public CityService(ApplicationDbContext db)
        {
            this._db = db;
        }

        public async Task<Guid> CreateCity(string city)
        {
            var citycreation = new City { CityName = city };   

            await _db.Cities.AddAsync(citycreation);

            await _db.SaveChangesAsync();

            return citycreation.Id;
        }

        public async Task<bool> DeleteCity(Guid cityId)
        {
            var city = await _db.Cities.FirstOrDefaultAsync(c => c.Id == cityId);

            if (city == null)
                throw new NotFoundException("city not found");

            _db.Cities.Remove(city);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateCity(Guid cityId, string cityName)
        {
            var city = await _db.Cities.FirstOrDefaultAsync(c => c.Id == cityId);

            if (city == null)
                throw new NotFoundException("city not found");

            city.CityName = cityName;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<List<City>> GetCities()
        {
            return await _db.Cities.ToListAsync();
        }
    }
}
