using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



using System;

namespace Mhrm.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int Request_by { get; set; }
        public int LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int? ApprovedBy { get; set; }

        
        
    }
}
