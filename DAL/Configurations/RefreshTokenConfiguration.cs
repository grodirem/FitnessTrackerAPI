﻿using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.DAL.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .IsRequired();

        builder.Property(rt => rt.Expires)
            .IsRequired();

        builder.HasOne(rt => rt.User)
            .WithOne(u => u.RefreshToken)
            .HasForeignKey<RefreshToken>(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}