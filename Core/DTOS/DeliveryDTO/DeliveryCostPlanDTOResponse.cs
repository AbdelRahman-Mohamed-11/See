using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.DeliveryDTO
{
    public class DeliveryCostPlanDTOResponse : BaseEntity
    {
        public string DeliveryName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime EffectiveDate { get; set; }

        public decimal DefaultDeliveryCost { get; set; }

        public double FreeDeliveryLimit { get; set; }

        public List<CreateDeliveryCostDetailDto> DeliveryCostDetails { get; set; }
    }
}
