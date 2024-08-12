using Application.Interfaces;
using AutoMapper;
using Core.DTOS;
using Core.Entities.Prescription_Lenses;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _map;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrescriptionService(ApplicationDbContext db , IMapper map , UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _map = map;
            this._userManager = userManager;
        }
        public async Task<Guid?> CreatePrescirpitionRanges(
            PriceRangesDtoRequest priceRanges)
        {
            var presc = new PriceRange
            {
                ApplicationManagerID = priceRanges.ApplicationManagerID.Value,
                CoatingTypeID = priceRanges.CoatingTypeID,
                LensTypeID = priceRanges.LensTypeID,
                VendorCountryID = priceRanges.VendorCountryID,
                Price = priceRanges.Price,
                CylinderMax = priceRanges.CylinderMax,
                CylinderMin = priceRanges.CylinderMin,
                SphereMax = priceRanges.SphereMax,
                SphereMin = priceRanges.SphereMin
            };

            await _db.PriceRanges.AddAsync(presc);

            await _db.SaveChangesAsync();

            return presc.Id;
        }

        public async Task<Guid?> CreatePrescriptionForUser(UserPrescriptionRequest PrescriptionUserDto)
        {

            var userPrescription = _map.Map<UserPrescription>(PrescriptionUserDto);

            var product = await _db.Products.FirstOrDefaultAsync(p =>
                p.Id == PrescriptionUserDto.ProductId);

            if (product == null)
            {
                throw new Exception("Associated product not found for the prescription"); 
            }

            var priceRange = await _db.PriceRanges.FirstOrDefaultAsync(pr =>
                pr.ApplicationManagerID == product.ManagerId &&
                pr.SphereMin <= userPrescription.DistanceSphereRight &&
                pr.SphereMax >= userPrescription.DistanceSphereRight &&
                pr.CylinderMin <= userPrescription.DistanceCylinderRight &&
                pr.CylinderMax >= userPrescription.DistanceCylinderRight &&
                pr.CoatingTypeID == userPrescription.CoatingTypeID &&
                pr.VendorCountryID == userPrescription.VendorCountryID);

            // If price range found, assign the price to prescriptionPrice
            if (priceRange != null)
            {
                userPrescription.Price = priceRange.Price;
            }
            else
            {
                throw new Exception("Price range not found for the selected prescription"); // Or handle differently
            }

            await _db.UserPrescriptions.AddAsync(userPrescription);
            
            await _db.SaveChangesAsync();

            return userPrescription.Id;

        }


        public async Task<bool> DeletePrescriptionRanges(Guid? id)
        {
            var priceRange = await _db.PriceRanges.FirstOrDefaultAsync(p => p.Id == id);

            if (priceRange == null)
                return false;

            _db.PriceRanges.Remove(priceRange);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditPrescriptionRanges(PriceRangesDtoEdit rangesDtoEdit)
        {
            var prescriptionDetails = await _db.PriceRanges.FirstOrDefaultAsync(p => p.Id == rangesDtoEdit.Id);

            if(prescriptionDetails == null)
                return false;

            prescriptionDetails.SphereMin = rangesDtoEdit.SphereMin;
            prescriptionDetails.SphereMax = rangesDtoEdit.SphereMax;
            prescriptionDetails.CylinderMax = rangesDtoEdit.CylinderMax;
            prescriptionDetails.CylinderMin = rangesDtoEdit.CylinderMin;
            prescriptionDetails.CoatingTypeID = rangesDtoEdit.CoatingTypeID;
            prescriptionDetails.VendorCountryID = rangesDtoEdit.CountryID;
            prescriptionDetails.LensTypeID = rangesDtoEdit.LensTypeID;
            prescriptionDetails.Price = rangesDtoEdit.Price;

            await _db.SaveChangesAsync();

            return true;
        }


        public async Task<UserPrescriptionDtoResponse> GetLastestUserPrescription(Guid userId)
        {
            var userPrescription = await _db.UserPrescriptions
                .Where(p => p.UserID == userId)
                .OrderByDescending(p => p.DateOfPrescirpiton)
                .FirstOrDefaultAsync();

            return _map.Map<UserPrescriptionDtoResponse>(userPrescription);
        }


        public async Task<IReadOnlyList<PriceRangeDtoResponse>> 
            GetManagerPrescriptionRanges(Guid id)
        {
            var priceRanges =  await _db.PriceRanges.Where(p => p.ApplicationManagerID == id).ToListAsync();

            return _map.Map<List<PriceRangeDtoResponse>>(priceRanges);
        }

        public async Task<UserPrescriptionDtoResponse?> GetPrescription(
            Guid prescriptionId)
        {
            var userPrescription = await _db.UserPrescriptions
               .FirstOrDefaultAsync(p => p.Id == prescriptionId);

            return _map.Map<UserPrescriptionDtoResponse>(userPrescription);
        }

        public async Task<UserPrescriptionDtoResponse> GetUserPrescription(Guid userId)
        {
            var userPrescription = await _db.UserPrescriptions
                .FirstOrDefaultAsync(p => p.UserID == userId);
                
            return _map.Map<UserPrescriptionDtoResponse>(userPrescription);
        }

        public async Task<IReadOnlyList<VendorCountry>> GetVendors()
        {
            return await _db.VendorCountries.ToListAsync();
        }

        public async Task<IReadOnlyList<LensType>> GetLensesTypes()
        {
            return await _db.LensType.ToListAsync();
        }

        public async Task<IReadOnlyList<CoatingType>> GetCoatings()
        {
            return await _db.CoatingTypes.ToListAsync();
        }

        public async Task<IReadOnlyList<PriceResponse>> GetVendorPricesForPrescription(
           [FromBody] UserPrescriptionRequest prescriptionRequest , Guid managerId)
        {
            var priceRanges = await _db.PriceRanges
                .Include(p => p.VendorCountry)
            .Where(pr => pr.ApplicationManagerID == managerId &&
                         pr.SphereMin <= prescriptionRequest.DistanceSphereRight &&
                         pr.SphereMax >= prescriptionRequest.DistanceSphereRight &&
                         pr.CylinderMin <= prescriptionRequest.DistanceCylinderRight &&
                         pr.CylinderMax >= prescriptionRequest.DistanceCylinderRight &&
                         pr.CoatingTypeID == prescriptionRequest.CoatingTypeID
                         && pr.LensTypeID == prescriptionRequest.LensTypeID 
                        )
            .ToListAsync();


            return priceRanges.Select(p => new PriceResponse
            {
                Price = p.Price,
                VendorId = p.VendorCountryID,
                vendorName = p.VendorCountry.VendorCountryName
            }).ToList();
        }
    }
}
