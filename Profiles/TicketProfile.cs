using AutoMapper;
using EvoTicketing.DTOs.Incoming;
using EvoTicketing.DTOs.OutGoing;
using EvoTicketing.Models;

namespace EvoTicketing.Profiles;

public class TicketProfile : Profile
{
    public TicketProfile()
    {
        CreateMap<NewTicketDTO, TicketsDTO>();
        CreateMap<Ticket, TicketsDTO>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.TicketCode, opt => opt.MapFrom(src => src.TicketCode))
        .ForMember(dest => dest.IssuerName, opt => opt.MapFrom(src => src.IssuerName))
        .ForMember(dest => dest.Occasion, opt => opt.MapFrom(src => src.Occasion))
        .ForMember(dest => dest.Benefeciary, opt => opt.MapFrom(src => src.Benefeciary))
        .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
        .ForMember(dest => dest.ValidUntil, opt => opt.MapFrom(src => src.ValidUntil));

        CreateMap<NewTicketDTO, Ticket>()
        .ForMember(dest => dest.TicketCode, opt => opt.MapFrom(src => src.TicketCode))
        .ForMember(dest => dest.IssuerName, opt => opt.MapFrom(src => src.IssuerName))
        .ForMember(dest => dest.Occasion, opt => opt.MapFrom(src => src.Occasion))
        .ForMember(dest => dest.Benefeciary, opt => opt.MapFrom(src => src.Benefeciary))
        .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
        .ForMember(dest => dest.ValidUntil, opt => opt.MapFrom(src => DateTime.UtcNow.AddDays(3)))
        .ForMember(dest => dest.Valid, opt => opt.MapFrom(src => 1))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForMember(dest => dest.UpDatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}