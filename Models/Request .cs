using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Mhrm.Models
{

    [Table("requests")]
    public class Request
    {
        [Key]
        public int Id { get; set; }
        public int RequestBy { get; set; }
        public bool LeaveRequest { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int ApprovedBy { get; set; }
    }
}