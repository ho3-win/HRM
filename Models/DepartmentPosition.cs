using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Mhrm.Models
{
    public class DepartmentPosition
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }

         
        public virtual Department Department { get; set; } = default!;
        public virtual Position Position { get; set; } = default!;
        
        
    }
}