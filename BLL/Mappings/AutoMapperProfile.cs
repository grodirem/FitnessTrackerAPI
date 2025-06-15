using AutoMapper;
using BLL.DTOs.Goal;
using BLL.DTOs.Integration;
using BLL.DTOs.User;
using BLL.DTOs.Workout;
using DAL.Entities;

namespace BLL.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserRegisterDto, User>();
        CreateMap<User, UserProfileDto>();
        CreateMap<UserUpdateDto, User>();

        CreateMap<WorkoutCreateDto, Workout>();
        CreateMap<WorkoutUpdateDto, Workout>();
        CreateMap<Workout, WorkoutResponseDto>();

        CreateMap<GoalSetDto, Goal>();
        CreateMap<Goal, GoalResponseDto>();

        CreateMap<ExternalWorkoutDto, WorkoutCreateDto>();
    }
}