using Core.DTOS;
using Core.Entities.Prescription_Lenses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class ApplicationManager : ApplicationUser
    {
        public string storeName { get; set; }

        public string BusinessLocation { get; set; }

        public PaymentInfo paymentInfo { get; set; }   

        public List<PriceRange> PriceRanges { get; set; } = new List<PriceRange>();
    }

  
}
