using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mhrm.Models
{

    public enum ContractStatus
    {
        Draft = 0,
        Active = 1,
        Expired = 2,
        Terminated = 3
    }

    [Table("contracts")]
    public class Contract
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Salary { get; set; }
        public int HoursPerWeek { get; set; }
        public string ContractType { get; set; }
        public string Status { get; set; }
    }
}