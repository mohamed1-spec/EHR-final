using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EHR.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        [Required] public int PatientId { get; set; }
        [ForeignKey(nameof(PatientId))] public Patient Patient { get; set; } = default!;
        [Required] public int DoctorId { get; set; }
        [ForeignKey(nameof(DoctorId))] public Doctor Doctor { get; set; } = default!;
        [Required] public DateTime StartTime { get; set; }
        [Required] public DateTime EndTime { get; set; }
        [Required, MaxLength(20)] public string Status { get; set; } = "Scheduled";
    }
}