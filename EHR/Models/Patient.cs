using System;
using System.ComponentModel.DataAnnotations;
namespace EHR.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        [Required, MaxLength(50)] public string FirstName { get; set; } = default!;
        [Required, MaxLength(50)] public string LastName { get; set; } = default!;
        [Required] public DateTime DateOfBirth { get; set; }
        [MaxLength(10)] public string? Gender { get; set; }
        [MaxLength(200)] public string? Email { get; set; }
    }
}
