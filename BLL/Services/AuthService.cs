using AutoMapper;
using BLL.DTOs.Auth;
using BLL.DTOs.RefreshToken;
using BLL.DTOs.User;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Contexts;
using DAL.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Data.Entity;
using ValidationException = BLL.Exceptions.ValidationException;

namespace BLL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly FitnessTrackerContext _context;
    private readonly IValidator<UserLoginDto> _loginValidator;
    private readonly IValidator<UserRegisterDto> _registerValidator;

    public AuthService(
        UserManager<User> userManager,
        IConfiguration configuration,
        IMapper mapper,
        ITokenService tokenService,
        FitnessTrackerContext context,
        IValidator<UserLoginDto> loginValidator,
        IValidator<UserRegisterDto> registerValidator)
    {
        _userManager = userManager;
        _configuration = configuration;
        _mapper = mapper;
        _tokenService = tokenService;
        _context = context;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
    }

    public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _loginValidator.ValidateAsync(loginDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            throw new UnauthorizedException("Invalid password");
        }

        return await GenerateAuthResponse(user);
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
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            throw new ValidationException("Refresh token is required");
        }

        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken == null)
        {
            throw new NotFoundException("Refresh token not found");
        }

        if (refreshToken.Expires < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token has expired");
        }

        return await GenerateAuthResponse(refreshToken.User);
    }

    public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == userId, cancellationToken);

        if (refreshToken != null)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<AuthResponseDto> GenerateAuthResponse(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateJwtToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var tokenExpiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"]));

        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == user.Id);

        if (existingToken != null)
        {
            _context.RefreshTokens.Remove(existingToken);
        }

        var newRefreshToken = new RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Token = token,
            RefreshToken = refreshToken,
            TokenExpires = tokenExpiry,
            UserProfile = _mapper.Map<UserProfileDto>(user)
        };
    }
}