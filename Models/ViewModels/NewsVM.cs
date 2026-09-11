public class NewsVM
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public string EmployeeFirstName { get; set; }
    public string EmployeeLastName { get; set; }
    public int EmployeeId { get; set; }

    public bool IsImportant { get; set; }

    public int Category { get; set; }
}