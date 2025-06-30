using AutoMapper;
using BLL.DTOs.Goal;
using DAL.Entities;

namespace BLL.Mappings;

public class GoalMappings : Profile
{
    public GoalMappings()
    {
        CreateMap<GoalSetDto, Goal>();
        CreateMap<Goal, GoalResponseDto>();
        CreateMap<Goal, GoalProfileDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Active));
    }
}
