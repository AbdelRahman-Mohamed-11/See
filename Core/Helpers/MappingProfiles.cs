using AutoMapper;
using Core.DTOS;
using Core.DTOS.DeliveryDTO;
using Core.DTOS.ProductDTOS;
using Core.Entities;
using Core.Entities.Delivery;
using Core.Entities.Prescription_Lenses;
using Core.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() {

            CreateMap<ProductDTORequest, Product>()
                   .ForMember(dest => dest.ProductColors, opt => opt.Ignore())
                   .ForMember(dest => dest.ProductGenderTypes, opt => opt.Ignore())
                   .ForMember(dest => dest.PicturesUrl, opt => opt.Ignore());


            CreateMap<LensDtoRequest, Lense>()
                   .ForMember(dest => dest.ProductColors, opt => opt.Ignore())
                   .ForMember(dest => dest.ProductGenderTypes, opt => opt.Ignore())
                   .ForMember(dest => dest.PicturesUrl, opt => opt.Ignore());



            CreateMap<GlassDtoRequest, Glass>()
                    .ForMember(dest => dest.ProductColors, opt => opt.Ignore())
                    .ForMember(dest => dest.ProductGenderTypes, opt => opt.Ignore())
                    .ForMember(dest => dest.PicturesUrl, opt => opt.Ignore());


            CreateMap<AccessoryDtoRequest, Accessory>()
                .ForMember(dest => dest.ProductColors, opt => opt.Ignore())
                .ForMember(dest => dest.ProductGenderTypes, opt => opt.Ignore())
                .ForMember(dest => dest.PicturesUrl, opt => opt.Ignore());




            CreateMap<BaseEntity , AccessoryDTOResponse>();

            CreateMap<BaseEntity , GlassDTOResponse>();

            CreateMap<BaseEntity , LensesDTOResponse>();


            CreateMap<DeliveryCostPlanSetup, DeliveryCostPlanDTOResponse>();

            CreateMap<CreateDeliveryCostDetailDto,DeliveryCostDetail>();
          
            CreateMap<DeliveryCostDetail, CreateDeliveryCostDetailDto>();

            CreateMap<UpdateDeliveryCostPlanSetup, DeliveryCostPlanDTOResponse>();

            CreateMap<PriceRangesDtoRequest, PriceRange>();

            CreateMap<UserPrescription , UserPrescriptionDtoResponse>();


            CreateMap<UserPrescriptionRequest, UserPrescription>();
        }
    }
}
