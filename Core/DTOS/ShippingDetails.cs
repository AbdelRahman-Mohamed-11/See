using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class ShippingDetails
    {
        public string Notes { get; set; }
        public int NumberOfPackages { get; set; }
        public int Weight { get; set; }
        public string WeightUnit { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Contents { get; set; }
    }
}
