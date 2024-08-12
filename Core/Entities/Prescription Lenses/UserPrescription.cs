using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Prescription_Lenses
{
    public class UserPrescription : BaseEntity
    {
        //public Guid PrescriptionTypeID { get; set; }
        
        //public PrescriptionType PrescriptionType { get; set; }

        public Guid? UserID { get; set; } 
       
        public Guid LensTypeID { get; set; }
        
        public LensType LensType { get; set; }

        public Guid CoatingTypeID { get; set; }
        public CoatingType CoatingType { get; set; }


        public Guid VendorCountryID { get; set; }
        public VendorCountry Country { get; set; }

        public DateTime DateOfPrescirpiton { get; set; } = DateTime.UtcNow;
         

        public decimal? DistanceSphereRight { get; set; }
        public decimal? DistanceSphereLeft { get; set; } 
        public decimal? DistanceCylinderRight { get; set; }
        public decimal? DistanceCylinderLeft { get; set; }
        public decimal? DistanceRAxis { get; set; }
        public decimal? DistanceLAxis { get; set; }

        public decimal? RightAddition { get; set; }
        public decimal? LeftAddition { get; set; }
        
        public decimal Price { get; set; }
    }

}
