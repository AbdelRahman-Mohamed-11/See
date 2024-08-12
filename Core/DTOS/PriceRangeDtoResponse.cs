using Core.Entities.Prescription_Lenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class PriceRangeDtoResponse
    {
        public PrescriptionType PrescriptionType { get; set; }
        public Guid PrescriptionTypeID { get; set; }

        public LensType LensType { get; set; }
        public Guid LensTypeID { get; set; }

        public CoatingType CoatingType { get; set; }
        public Guid CoatingTypeID { get; set; }

        public VendorCountry VendorCountry { get; set; }
        public Guid CountryID { get; set; }

        public decimal SphereMin { get; set; }
        public decimal SphereMax { get; set; }
        public decimal CylinderMin { get; set; }
        public decimal CylinderMax { get; set; }
        public decimal Price { get; set; }
    }
}
