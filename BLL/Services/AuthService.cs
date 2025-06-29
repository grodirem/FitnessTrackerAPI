using AutoMapper;
using BLL.DTOs.Auth;
using BLL.DTOs.RefreshToken;
using BLL.DTOs.User;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Data.Entity;

namespace BLL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IValidator<UserLoginDto> _loginValidator;
    private readonly IValidator<UserRegisterDto> _registerValidator;

    public AuthService(
        UserManager<User> userManager,
        IMapper mapper,
        ITokenService tokenService,
        IValidator<UserLoginDto> loginValidator,
        IValidator<UserRegisterDto> registerValidator)
    {
        _userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
    }

    public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto, CancellationToken cancellationToken = default)
    {
        await _loginValidator.ValidateAsync(loginDto, cancellationToken);

        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            throw new UnauthorizedException("Invalid password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateJwtToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        var response = _mapper.Map<AuthResponseDto>(user);
        response.Token = token;
        response.RefreshToken = refreshToken;
        response.IsAuthenticated = true;
        return response;
    }

    public async Task<RegistrationResponseDto> RegisterAsync(UserRegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _registerValidator.ValidateAsync(registerDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new RegistrationResponseDto
            {
                IsRegistered = false,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        }

        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new ConflictException("Email already exists");
        }

        var user = _mapper.Map<User>(registerDto);
        user.UserName = registerDto.Email;

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return new RegistrationResponseDto
            {
                IsRegistered = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        await _userManager.AddToRoleAsync(user, "User");
        return new RegistrationResponseDto { IsRegistered = true };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .ToAsyncEnumerable()
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthResponseDto
            {
                ErrorMessage = "The refresh token is invalid."
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _tokenService.GenerateJwtToken(user, roles);

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Token = newAccessToken,
            RefreshToken = request.RefreshToken
        };
    }

    public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _userManager.UpdateAsync(user);
    }
}