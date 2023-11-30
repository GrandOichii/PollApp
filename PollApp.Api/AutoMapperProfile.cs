using System.Reflection.PortableExecutable;
using AutoMapper;

namespace PollApp.Api;

public class AutoMapperProfile : Profile {
    public AutoMapperProfile()
    {
        CreateMap<Poll, GetPollDto>();
        CreateMap<AddPollDto, Poll>();

        CreateMap<PollOption, GetPollOptionDto>();
        CreateMap<AddPollOptionDto, PollOption>();

        CreateMap<User, GetUserDto>();
    }
}