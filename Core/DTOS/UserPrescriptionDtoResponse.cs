using Core.Entities.Prescription_Lenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class UserPrescriptionDtoResponse
    {
        public string PrescriptionType { get; set; }
        public Guid PrescriptionTypeId { get; set; }
        public string LensType { get; set; }
        public Guid LensTypeId { get; set; }
        public string CoatingType { get; set; }
        public Guid CoatingTypeId { get; set; }

        public string Country { get; set; }
        public Guid CountryTypeId { get; set; }
        public DateTime DateOfPrescirpiton { get; set; } = DateTime.UtcNow;
        public decimal Price { get; set; }
        public decimal? DistanceSphereRight { get; set; }
        public decimal? DistanceSphereLeft { get; set; }
        public decimal? DistanceCylinderRight { get; set; }
        public decimal? DistanceCylinderLeft { get; set; }
        public decimal? DistanceRAxis { get; set; }
        public decimal? DistanceLAxis { get; set; }

        public decimal? RightAddition { get; set; }
        public decimal? LeftAddition { get; set; }
    }
}
