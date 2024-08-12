using Application.Interfaces;
using Core.DTOS.DeliveryDTO;
using Core.Entities.Delivery;
using Infrastructure.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlassesApp.Controllers
{
    
    public class DeliveryController : BaseApiController
    {
        private readonly DeliveryCostPlanInterface _deliveryService;

        public DeliveryController(DeliveryCostPlanInterface
            deliveryService)
        {
            _deliveryService = deliveryService;
        }

        [HttpPost("createDeliveryPlan")]

         public async Task<ActionResult<CreateDeliveryCostPlanSetup>>
            CreateDeliveryCostDetailPlan([FromBody] 
        CreateDeliveryCostPlanSetup deliveryPlan)
           {
               return Ok(await _deliveryService.Create(deliveryPlan));
           }

        [HttpGet("getAllDeliveries")]
        public async Task<ActionResult<List<DeliveryCostPlanDTOResponse>>> 
            GetDeliveryPlans()
        {
            return Ok(await _deliveryService.GetDeliveryPlans());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryCostPlanDTOResponse>> GetDeliveryPlanById(Guid id)
        {
            return Ok(await _deliveryService.GetDeliveryPlan(id));
        }

        

        [HttpGet("getEffectiveCostPlan")]

        public async Task<ActionResult<DeliveryCostPlanDTOResponse>> GetEffectiveCostPlan()
        {
            return await _deliveryService.GetEffectiveCostPlan();
        }

        [HttpPut("{id}")]

        public async Task<ActionResult<DeliveryCostPlanDTOResponse>>
            UpdateDeliveryPlan(UpdateDeliveryCostPlanSetup deliveryUpdate)
        {
            return Ok(await _deliveryService.UpdateDeliveryPlan(deliveryUpdate));
        }


    }
}
