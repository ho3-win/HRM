using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Mhrm.Models
{
    [Table("Tab_User")]
    public class User
    {
        
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [Required]
        [MaxLength(25)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; } 
        [Required]
        public int RoleId { get; set; }
        public string Bio { get; set; }
   
        public Employee? Employee { get; set; } = default!;

    }
}
