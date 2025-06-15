using AutoMapper;
using BLL.DTOs.User;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Contexts;
using DAL.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ValidationException = BLL.Exceptions.ValidationException;

namespace BLL.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IValidator<UserUpdateDto> _updateValidator;
    private readonly FitnessTrackerContext _context;

    public UserService(
        UserManager<User> userManager,
        IMapper mapper,
        IValidator<UserUpdateDto> updateValidator,
        FitnessTrackerContext context)
    {
        _userManager = userManager;
        _mapper = mapper;
        _updateValidator = updateValidator;
        _context = context;
    }

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task UpdateUserProfileAsync(Guid userId, UserUpdateDto updateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (!string.IsNullOrEmpty(updateDto.Name))
        {
            user.Name = updateDto.Name;
        }

        if (updateDto.BirthDate != default)
        {
            user.BirthDate = updateDto.BirthDate;
        }

        if (updateDto.Gender.HasValue)
        {
            user.Gender = updateDto.Gender.Value;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to update user profile");
        }
    }

    public async Task DeleteAccountAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(refreshTokens);

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to delete user account");
        }

        await _context.SaveChangesAsync();
    }
}