using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Mhrm.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<DepartmentPosition> DepartmentPosition { get; set; }
            = new List<DepartmentPosition>();

        
    }
}
