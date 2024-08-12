using Application.Interfaces;
using Core.DTOS;
using Core.Entities.Prescription_Lenses;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GlassesApp.Controllers
{
    public class PrescriptionController : BaseApiController
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly ApplicationDbContext _db;
        public PrescriptionController(
            IPrescriptionService prescriptionService,
            ApplicationDbContext db)
        {
            this._prescriptionService = prescriptionService;
            this._db = db;
        }

        [Authorize("Manager")]
        [HttpPost("create-price-range")]
        public async Task<ActionResult<Guid?>> CreatePriceRangesForLense(PriceRangesDtoRequest priceRanges)
        {
            var userId = User.FindFirst("ID")?.Value;

            priceRanges.ApplicationManagerID = Guid.Parse(userId!);

            var prescriptionRangeId =
                await _prescriptionService.CreatePrescirpitionRanges(priceRanges);
            return Ok(prescriptionRangeId);
        }

        [Authorize(Roles ="AppUser")]
        [HttpPost("create-user-prescription")]
        public async Task<ActionResult<Guid>> CreateUserPrescription(UserPrescriptionRequest userPrescriptionRequest)
        {
            var prescriptionId = await _prescriptionService.CreatePrescriptionForUser(userPrescriptionRequest);
            return Ok(prescriptionId);
        }

        [Authorize(Roles ="Manager")]
        [HttpDelete("delete-price-range/{id}")]
        public async Task<ActionResult<bool>> DeletePriceRange(Guid id)
        {
            var success = await _prescriptionService.DeletePrescriptionRanges(id);

            if (!success)
                return NotFound("prescription not found");

            return Ok(success);
        }

        [Authorize(Roles= "Manager")]
        [HttpPut("edit-price-range")]
        public async Task<ActionResult<bool>> EditPriceRange(PriceRangesDtoEdit rangesDtoEdit)
        {
            var success = await _prescriptionService.EditPrescriptionRanges(rangesDtoEdit);

            if (!success)
                return NotFound("prescription not found");

            return Ok(success);
        }

        [Authorize(Roles = "AppUser")]
        [HttpGet("get-user-prescription/{userId}")]
        public async Task<ActionResult<UserPrescriptionDtoResponse>> GetUserPrescription(Guid userId)
        {
            var userPrescription = await _prescriptionService.GetUserPrescription(userId);
            return Ok(userPrescription);
        }

        [Authorize(Roles = "AppUser")]
        [HttpGet("get-latest-user-prescription/{userId}")]
        public async Task<ActionResult<UserPrescriptionDtoResponse>> GetLatestUserPrescription(Guid userId)
        {
            var userPrescription = await _prescriptionService.GetLastestUserPrescription(userId);
            return Ok(userPrescription);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("get-manager-prescription-ranges/{id}")]
        public async Task<ActionResult<IReadOnlyList<PriceRangeDtoResponse>>> GetManagerPrescriptionRanges(Guid id)
        {
            var priceRanges = await _prescriptionService.GetManagerPrescriptionRanges(id);
            return Ok(priceRanges);
        }

        [HttpGet("get-vendors")]
        public async Task<ActionResult<IReadOnlyList<VendorCountry>>> GetVendors()
        {
            return Ok(await _prescriptionService.GetVendors());
        }

        [HttpGet("get-lenses")]
        public async Task<ActionResult<IReadOnlyList<LensType>>> GetLenses()
        {
            return Ok(await _prescriptionService.GetLensesTypes());
        }

        [HttpGet("get-coating")]
        public async Task<ActionResult<IReadOnlyList<CoatingType>>> GetCoating()
        {
            return Ok(await _prescriptionService.GetCoatings());
        }

        [Authorize(Roles = "AppUser")]
        [HttpGet("get-prescription/{id}")]
        public async Task<ActionResult<UserPrescriptionDtoResponse>> GetPrescriptionById(Guid id)
        {
            var prescription = await _prescriptionService.GetPrescription(id);
            if (prescription == null)
                return NotFound("Prescription not found");

            return Ok(prescription);
        }

        [Authorize(Roles = "AppUser")]
        [HttpPost("get-vendor-prices-for-prescription/{managerId}")]
        public async Task<ActionResult<IReadOnlyList<PriceResponse>>> GetVendorPricesForPrescription(
            [FromBody] UserPrescriptionRequest prescriptionRequest, Guid managerId)
        {
            var vendorPrices = await _prescriptionService.GetVendorPricesForPrescription(prescriptionRequest, managerId);
            return Ok(vendorPrices);
        }
    }
}
