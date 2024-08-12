using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class OrderBasketDTO
    {             
       public string firstName { get; set; }

       public string lastName { get; set; }
        
       public Guid basketId { get; set; }
       
       public string ? street { get; set; }

       public string? newPhoneNumber { get; set; }

       public Guid? ExistingPhoneNumber { get; set; }
       
       public Guid? addressId { get; set; }

       public Guid? CityId { get; set; }
        
       public bool? IsAndroidDevice { get; set; }  
    }
}
