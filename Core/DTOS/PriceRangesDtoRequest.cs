using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class PriceRangesDtoRequest
    {
        public Guid? ApplicationManagerID { get; set; }
        public Guid LensTypeID { get; set; }
        public Guid CoatingTypeID { get; set; }
        public Guid VendorCountryID { get; set; }
        public decimal SphereMin { get; set; }
        public decimal SphereMax { get; set; }
        public decimal CylinderMin { get; set; }
        public decimal CylinderMax { get; set; }
        public decimal Price { get; set; }
    }
}
