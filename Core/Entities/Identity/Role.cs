using System;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities.Identity
{
    public class Role :IdentityRole<Guid>
    {
        public Role() : base()
        { }
        public Role(string roleName) : base(roleName)
        {
        }
        public const string Admin = "Admin";
        public const string Trainee = "Trainee";
        public const string Trainer = "Trainer";
    }
}