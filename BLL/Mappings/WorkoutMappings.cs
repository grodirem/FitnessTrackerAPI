using AutoMapper;
using BLL.DTOs.Integration;
using BLL.DTOs.Workout;
using DAL.Entities;

namespace BLL.Mappings;

public class WorkoutMappings : Profile
{
    public WorkoutMappings()
    {
        CreateMap<WorkoutCreateDto, Workout>();
        CreateMap<WorkoutUpdateDto, Workout>()
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes ?? string.Empty));

        CreateMap<Workout, WorkoutResponseDto>();

        CreateMap<ExternalWorkoutDto, Workout>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.StartTime.Date))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes ?? string.Empty));

        CreateMap<ExternalWorkoutDto, WorkoutCreateDto>();
    }
}
