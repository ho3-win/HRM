using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Mhrm.Models
{
    public class Document
    {
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    
    public Employee? Employee { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public string Title { get; set; } = default!;
    public int FileType { get; set; }

    public string FilePath { get; set; } = default!;
    public DateTime UploadAt { get; set; }

    public string? Description { get; set; }
   }

}