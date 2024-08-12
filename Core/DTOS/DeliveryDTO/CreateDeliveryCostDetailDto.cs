using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.DeliveryDTO
{
    public class CreateDeliveryCostDetailDto
    {
        [Required]
        public decimal Price { get; set; }

        [Required]
        public Guid CityId { get; set; }        
    }
}
