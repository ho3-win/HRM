using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Mhrm.Models
{
  public class News
  {
    public int Id { get; set; }

    public string? FilePath { get; set; }
    
    public int? PublishedBy { get; set; }

    public string Title { get; set; } 
    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }

    public bool IsActive { get; set; }
    public bool IsImportant { get; set; }

    public int Category { get; set; }  

   
    
  }

}