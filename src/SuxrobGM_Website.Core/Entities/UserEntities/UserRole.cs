﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using SuxrobGM_Website.Core.Interfaces.Entities;

namespace SuxrobGM_Website.Core.Entities.UserEntities
{
    public enum Role
    {
        SuperAdmin,
        Admin,
        Moderator,
        Editor
    }

    public class UserRole : IdentityRole, IEntity<string>
    {
        public UserRole(Role role) : base(role.ToString())
        {
            Role = role;
        }

        [StringLength(32)]
        [Display(Name = "ID")]
        public sealed override string Id { get; set; }

        public Role Role { get; set; }

        [StringLength(250, ErrorMessage = "Characters must be less than 250")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
