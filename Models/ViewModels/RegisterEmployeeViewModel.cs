using System;
using System.ComponentModel.DataAnnotations;

namespace Mhrm.Models.ViewModels
{
    public class RegisterEmployeeViewModel
    {
        // Employee
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string National_id { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? HireDate { get; set; }

        public string InsuranceNumber { get; set; }

        public int DepartmentPositionId { get; set; }

        public int Status { get; set; }

        // User
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public int RoleId { get; set; }

        public string Bio { get; set; }
    }
}
