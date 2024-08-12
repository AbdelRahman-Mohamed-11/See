using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Products
{
    public class Lense : Product
    {
        // for colored lenses
        public double LensBaseCurve { get; set; }

        public double Lensdiameter { get; set; }

        public LensUsage LensUsage { get; set; }

        public double WaterContent { get; set; }

    }

}
