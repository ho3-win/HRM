using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mhrm.Models
{
    // public enum EmployeeStatus
    // {
    //     Inactive = 0,
    //     Active = 1,
    //     Suspended = 2,
    //     Resigned = 3
    // }
    public class Employee
    {
        public int Id { get; set; }
        public string National_id { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? HireDate { get; set; }
        public int Status { get; set; }
        public string InsuranceNumber { get; set; }
        [Required]
        public int DepartmentPositionId { get; set; }

        public DepartmentPosition? DepartmentPosition { get; set; }
       
        
        public User? User { get; set; }

    }
}