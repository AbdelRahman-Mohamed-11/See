using Core.Entities.Prescription_Lenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class UserPrescriptionRequest
    {
        public Guid LensTypeID { get; set; }

        public Guid CoatingTypeID { get; set; }

        public Guid VendorCountryID { get; set; }

        public DateTime DateOfPrescirpiton { get; set; } = DateTime.UtcNow;

        public decimal? DistanceSphereRight { get; set; }
        public decimal? DistanceSphereLeft { get; set; }
        public decimal? DistanceCylinderRight { get; set; }
        public decimal? DistanceCylinderLeft { get; set; }
        public decimal? DistanceRAxis { get; set; }
        public decimal? DistanceLAxis { get; set; }

        public decimal? RightAddition { get; set; }
        public decimal? LeftAddition { get; set; }

        public Guid? UserId { get; set; }

        public Guid ProductId { get; set; }
    }
}
