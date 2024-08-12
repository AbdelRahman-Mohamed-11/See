using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Order
{
    public class DeliveryPrice : BaseEntity
    {
        public string CityName { get; set; }

        public int CityPrice { get; set; }  
    }
}
