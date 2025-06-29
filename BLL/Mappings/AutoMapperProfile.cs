using AutoMapper;
using BLL.DTOs.Auth;
using BLL.DTOs.Goal;
using BLL.DTOs.Integration;
using BLL.DTOs.User;
using BLL.DTOs.Workout;
using DAL.Entities;
using System.Text.Json;

namespace BLL.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserRegisterDto, User>();
        CreateMap<UserUpdateDto, User>();
        CreateMap<User, AuthResponseDto>();

        CreateMap<UserProfileDto, User>()
            .ForMember(dest => dest.NotificationSettingsJson,
                opt => opt.MapFrom(src =>
                    JsonSerializer.Serialize(
                        src.NotificationSettings ?? new NotificationSettingsDto(),
                        new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })));

        CreateMap<User, UserProfileDto>()
            .ForMember(dest => dest.NotificationSettings,
                opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.NotificationSettingsJson)
                        ? new NotificationSettingsDto()
                        : JsonSerializer.Deserialize<NotificationSettingsDto>(
                            src.NotificationSettingsJson,
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                          ?? new NotificationSettingsDto()));

        CreateMap<WorkoutCreateDto, Workout>();
        CreateMap<WorkoutUpdateDto, Workout>();
        CreateMap<Workout, WorkoutResponseDto>();

        CreateMap<GoalSetDto, Goal>();
        CreateMap<Goal, GoalResponseDto>();
        CreateMap<Goal, GoalProfileDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Active));

        CreateMap<ExternalWorkoutDto, WorkoutCreateDto>();
    }
}