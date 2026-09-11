using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mhrm.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ICollection<DepartmentPosition> DepartmentPosition { get; set; }
            = new List<DepartmentPosition>();

        
    }
}
