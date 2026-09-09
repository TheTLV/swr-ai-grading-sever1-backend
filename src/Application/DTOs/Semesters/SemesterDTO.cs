using Domain.Enums;

namespace Application.DTOs.Semesters;
public class SemesterDTO
{
    public Guid SemesterId { get; set; }
    public string SemesterCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SemesterStatus Status { get; set; }
}
