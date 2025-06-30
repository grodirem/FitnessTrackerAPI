using AutoMapper;
using BLL.DTOs.Auth;
using BLL.DTOs.User;
using BLL.Extensions;
using DAL.Entities;

namespace BLL.Mappings;

public class UserMappings : Profile
{
    public UserMappings()
    {
        CreateMap<UserRegisterDto, User>();
        CreateMap<UserUpdateDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, AuthResponseDto>();

        CreateMap<UserProfileDto, User>()
            .ForMember(dest => dest.NotificationSettingsJson,
                opt => opt.MapFrom(src => src.NotificationSettings.ToJson()));

        CreateMap<User, UserProfileDto>()
            .ForMember(dest => dest.NotificationSettings,
                opt => opt.MapFrom(src => src.NotificationSettingsJson.FromJson<NotificationSettingsDto>()));
    }
}
