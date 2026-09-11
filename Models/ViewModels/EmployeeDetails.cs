namespace Mhrm.Models.ViewModels
{
    public class EmployeeDetails
    {
        public Employee Employee { get; set; }
        public User? User { get; set; }

        public List<Contract>? Contracts { get; set; }
        public List<Document>? Documents { get; set; }
        public List<LeaveRequest>? LeaveRequests { get; set; }
        public List<Request>? Requests { get; set; }
        public List<UserSession>? Sessions { get; set; }
    }
}
