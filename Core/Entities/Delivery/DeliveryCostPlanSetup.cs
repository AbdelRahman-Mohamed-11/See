using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Delivery
{
    public class DeliveryCostPlanSetup : BaseEntity
    {
        [Required]
        public string DeliveryName { get; set; }

        public bool IsActive { get; set; }

        public DateTime EffectiveDate { get; set; }

        public decimal DefaultDeliveryCost { get; set; }

        public double FreeDeliveryLimit { get; set; }
        
        public List<DeliveryCostDetail> DeliveryCostDetails { get; set; }
    }
}
