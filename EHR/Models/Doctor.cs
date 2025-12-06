using System.ComponentModel.DataAnnotations;
namespace EHR.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        [Required, MaxLength(50)] public string FirstName { get; set; } = default!;
        [Required, MaxLength(50)] public string LastName { get; set; } = default!;
        [MaxLength(200)] public string? Email { get; set; }
        [MaxLength(25)] public string? PhoneNumber { get; set; }
        [MaxLength(100)] public string? Specialty { get; set; }
    }
}
