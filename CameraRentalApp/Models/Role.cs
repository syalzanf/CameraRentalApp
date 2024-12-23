﻿using System.ComponentModel.DataAnnotations;

namespace CameraRentalApp.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        // Relasi ke UserRoles
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
