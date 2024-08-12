using Core.DTOS;
using Core.Entities.Prescription_Lenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPrescriptionService
    {
        // manager
        public Task<Guid?> CreatePrescirpitionRanges(PriceRangesDtoRequest priceRanges);
        // manager
        public Task<bool> EditPrescriptionRanges(PriceRangesDtoEdit rangesDtoEdit);
        // manager
        public Task<bool> DeletePrescriptionRanges(Guid? id);

        // user
        public Task<Guid?> CreatePrescriptionForUser(
            UserPrescriptionRequest PrescriptionUserDto);

        public Task<UserPrescriptionDtoResponse> GetUserPrescription(Guid userId);

        public Task<UserPrescriptionDtoResponse?> GetPrescription(Guid prescriptionId);

        public Task<IReadOnlyList<PriceRangeDtoResponse>> GetManagerPrescriptionRanges(Guid id);

        public Task<IReadOnlyList<PriceResponse>> 
            GetVendorPricesForPrescription(UserPrescriptionRequest prescriptionRequest
            , Guid managerId);
        
        public Task<UserPrescriptionDtoResponse> GetLastestUserPrescription(Guid id);

        public Task<IReadOnlyList<VendorCountry>> GetVendors();

        public Task<IReadOnlyList<LensType>> GetLensesTypes();


        public Task<IReadOnlyList<CoatingType>> GetCoatings();

    }
}
