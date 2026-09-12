using Mhrm.Models;
using System.Collections.Generic;

namespace Mhrm.Models.ViewModels
{
    public class WorkerDashboardViewModel
    {
        public Employee Employee { get; set; } = default!;
        public User User { get; set; } = default!;
        public List<NewsVM> LatestNews { get; set; } = new();
    }
}
