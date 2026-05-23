using Application.DTOs.Identites.Booths;
using Application.DTOs.Identites;
using AutoMapper;
using Domain.Entities;

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
        }
    }
}