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
                .ForMember(dest => dest.FrameCount, opt => opt.MapFrom(src => src.TopicsFrames.Count));
            CreateMap<CreateTopicDto, Topic>();

            // Frame
            CreateMap<Frame, FrameDto>()
                .ForMember(dest => dest.Topics, opt => opt.MapFrom(src => src.TopicsFrames.Select(tf => new FrameTopicDto
                {
                    TopicId = tf.TopicId,
                    TopicName = tf.Topic.TopicName,
                    LayoutType = tf.Topic.layoutType
                })));
            CreateMap<CreateFrameDto, Frame>()
                .ForMember(dest => dest.TopicsFrames, opt => opt.Ignore());

            // Setting
            CreateMap<Setting, SettingDto>();
            CreateMap<CreateBoothSettingDto, Setting>();
        }
    }
}