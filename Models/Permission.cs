using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mhrm.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Permission_Key { get; set; }
        public string Descriptions { get; set; }
       
    }
}