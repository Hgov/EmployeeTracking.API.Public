﻿using EmployeeTracking.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace EmployeeTracking.Infrastructure.IdentitySettings.Validators
{
    public class UserValidator : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            var errors = new List<IdentityError>();

            if (user.UserName.Length < 6)
            {
                errors.Add(ErrorDescriber.UserNameLength());
            }
            if (user.Email.Substring(0, user.Email.IndexOf("@")) == user.UserName)
            {
                errors.Add(ErrorDescriber.UserNameContainsEmail());
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
