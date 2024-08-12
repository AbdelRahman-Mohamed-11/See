using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Delivery
{
    public class DeliveryCostDetail : BaseEntity
    {
        public decimal Price { get; set; }


        public DeliveryCostPlanSetup DeliveryCostPlanSetupFk { 
            get; set; }

        public Guid CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; }
    }
}
