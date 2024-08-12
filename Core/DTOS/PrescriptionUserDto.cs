using Core.Entities.Prescription_Lenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class PrescriptionUserDto
    {
        public Guid UserPrescriptionID { get; set; }

        public Guid PrescriptionTypeID { get; set; }

        public Guid LensTypeID { get; set; }

        public Guid CoatingTypeID { get; set; }

        public Guid VendorCountryID { get; set; }

        public DateTime? DateOfPrescirpiton { get; set; } = DateTime.UtcNow;

        public decimal Price { get; set; }


        public decimal? DistanceSphereRight { get; set; }
        public decimal? DistanceSphereLeft { get; set; }
        public decimal? DistanceCylinderRight { get; set; }
        public decimal? DistanceCylinderLeft { get; set; }
        public decimal? DistanceRAxis { get; set; }
        public decimal? DistanceLAxis { get; set; }

        public decimal? NearSphereRight { get; set; }
        public decimal? NearSphereLeft { get; set; }
        public decimal? NearCylinderRight { get; set; }
        public decimal? NearCylinderLeft { get; set; }
        public decimal? NearRAxis { get; set; }
        public decimal? NearLAxis { get; set; }

    }
}
