using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace EHR.Pages
{
    public class MyAppointmentsModel : PageModel
    {
        public List<Appointment> Appointments { get; set; } = new List<Appointment>
        {
            new Appointment { Department = "General Medicine", HealthIssue = "Fever", AppointmentTime = DateTime.Now.AddDays(1) },
            new Appointment { Department = "Ophthalmology", HealthIssue = "Eye Pain", AppointmentTime = DateTime.Now.AddDays(2) },
            new Appointment { Department = "Cardiology", HealthIssue = "Chest Pain", AppointmentTime = DateTime.Now.AddDays(3) }
        };

        public void OnGet()
        {
        }
    }

    public class Appointment
    {
        public string Department { get; set; }
        public string HealthIssue { get; set; }
        public DateTime AppointmentTime { get; set; }
    }
}
