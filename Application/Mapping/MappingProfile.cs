using Application.DTOs.Identites.Booths;
using Application.DTOs.Identites;
using AutoMapper;
using Domain.Entities;
using Application.DTOs;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Branch
            CreateMap<Branch, BranchDto>();
            CreateMap<CreateBranchDto, Branch>();

            // Booths
            CreateMap<Booths, BoothDto>();
            CreateMap<CreateBoothDto, Booths>();

            //Invoice 
            CreateMap<Invoice, InvoiceDto>();
            CreateMap<CreateInvoicesDto, Invoice>();

            // Voucher
            CreateMap<Voucher, VoucherDto>();
            CreateMap<CreateVoucherDto, Voucher>();

            // Topic
            CreateMap<Topic, TopicDto>() 
                .ForMember(dest => dest.FrameCount, opt => opt.MapFrom(src => src.Frames.Count));
            CreateMap<CreateTopicDto, Topic>();

            // Frame
            CreateMap<Frame, FrameDto>();
            CreateMap<CreateFrameDto, Frame>();
        }
    }
}