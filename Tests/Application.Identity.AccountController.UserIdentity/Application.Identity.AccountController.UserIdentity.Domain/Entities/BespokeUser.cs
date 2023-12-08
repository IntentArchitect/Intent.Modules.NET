using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Application.Identity.AccountController.UserIdentity.Domain.Entities
{
    public class BespokeUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpired { get; set; }
    }
}