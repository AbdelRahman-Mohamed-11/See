using Application.Interfaces;
using AutoMapper;
using AutoMapper.Internal.Mappers;
using Core.DTOS.DeliveryDTO;
using Core.Entities.Delivery;
using Infrastructure.Migrations;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DeliveryCostPlanService : DeliveryCostPlanInterface
    {
        private readonly ApplicationDbContext _db;
        public IMapper _map { get; }

        public DeliveryCostPlanService(ApplicationDbContext db , IMapper map)
        {
            this._db = db;
            _map = map;
        }


        public async Task<DeliveryCostPlanDTOResponse> Create(CreateDeliveryCostPlanSetup input)
        {
            List<DeliveryCostDetail> deliveryCostDetails = 
                input.DeliveryCostDetails.Select(d => new DeliveryCostDetail {
                    Price = d.Price,
                    CityId = d.CityId
                }).ToList();

            DeliveryCostPlanSetup plan = new DeliveryCostPlanSetup
            {
                IsActive = input.IsActive,
                FreeDeliveryLimit = input.FreeDeliveryLimit,
                DefaultDeliveryCost = input.DefaultDeliveryCost,
                DeliveryName = input.DeliveryName,
                EffectiveDate = input.EffectiveDate,
                DeliveryCostDetails = deliveryCostDetails
            };

            await _db.DeliveryCostPlanSetups.AddAsync(plan);

            await _db.SaveChangesAsync();

            var planDtoResponse = _map.Map<DeliveryCostPlanDTOResponse>(plan);

            return planDtoResponse;
        }

        public async Task<List<DeliveryCostPlanDTOResponse>> GetDeliveryPlans()
        {
            var deliveryPlans = await _db.DeliveryCostPlanSetups
             .Include(d => d.DeliveryCostDetails)
             .ToListAsync();

            return _map.Map<List<DeliveryCostPlanDTOResponse>>(deliveryPlans);
        }

        public async Task<DeliveryCostPlanDTOResponse> GetDeliveryPlan(Guid id)
        {
            var deliveryPlan = await _db.DeliveryCostPlanSetups
                                .Include(p => p.DeliveryCostDetails)
                                .FirstOrDefaultAsync(d => d.Id == id);
            
            return _map.Map<DeliveryCostPlanDTOResponse>(deliveryPlan);
        }

        public async Task<DeliveryCostPlanDTOResponse> UpdateDeliveryPlan(
            UpdateDeliveryCostPlanSetup updateDeliveryPlan)
        {
            var deliveryCostDetails = _map.Map<List<DeliveryCostDetail>>
                (updateDeliveryPlan.DeliveryCostDetails);

            var deliveryPlan = await _db.DeliveryCostPlanSetups
                 .Include(d => d.DeliveryCostDetails)
                .FirstOrDefaultAsync(p => p.Id == updateDeliveryPlan.Id);

            
            if (updateDeliveryPlan is null)
                return null;

            deliveryPlan.EffectiveDate = updateDeliveryPlan.EffectiveDate;
            
            deliveryPlan.FreeDeliveryLimit = updateDeliveryPlan.FreeDeliveryLimit;
            
            deliveryPlan.DeliveryName = updateDeliveryPlan.DeliveryName;
            
            deliveryPlan.DefaultDeliveryCost = updateDeliveryPlan.DefaultDeliveryCost;

            deliveryPlan.DeliveryCostDetails = deliveryCostDetails;

            await _db.SaveChangesAsync();

            return _map.Map<DeliveryCostPlanDTOResponse>(updateDeliveryPlan);
        }

        public async Task<DeliveryCostPlanDTOResponse> GetEffectiveCostPlan()
        {
            var effectiveCost = await _db.DeliveryCostPlanSetups
                               .Include(d => d.DeliveryCostDetails)
                               .Where(d => d.IsActive && d.EffectiveDate <= DateTime.UtcNow)
                               .OrderByDescending(d => d.EffectiveDate)
                               .FirstOrDefaultAsync();

            return _map.Map<DeliveryCostPlanDTOResponse>(effectiveCost);
        }
    }
}
