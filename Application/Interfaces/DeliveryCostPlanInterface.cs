using Core.DTOS.DeliveryDTO;
using Core.Entities.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface DeliveryCostPlanInterface
    {
        public Task<DeliveryCostPlanDTOResponse> Create(CreateDeliveryCostPlanSetup input);

        public Task<List<DeliveryCostPlanDTOResponse>> GetDeliveryPlans();

        public Task<DeliveryCostPlanDTOResponse> GetDeliveryPlan(Guid id);


        public Task<DeliveryCostPlanDTOResponse> UpdateDeliveryPlan(UpdateDeliveryCostPlanSetup updateDeliveryPlan);

        public Task<DeliveryCostPlanDTOResponse> GetEffectiveCostPlan();
    }
}
