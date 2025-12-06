using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EHR.Pages
{
    public class DoctorHomeModel : PageModel
    {
        private readonly string connectionString =
            "Server=DESKTOP-3CKHFVG\\SQLEXPRESS01;Database=EHR_DB;Trusted_Connection=True;";

        public List<AppointmentModelData> Appointments { get; set; } = new List<AppointmentModelData>();
        public string? Message { get; set; }

        public void OnGet()
        {
            if (TempData.ContainsKey("Success"))
                Message = TempData["Success"]?.ToString();
            else if (TempData.ContainsKey("Error"))
                Message = TempData["Error"]?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        A.AppointmentID, 
                        U.FirstName + ' ' + U.LastName AS PatientName,
                        A.HealthIssue, 
                        A.Department, 
                        A.AppointmentDate, 
                        A.Status
                    FROM Appointments A
                    INNER JOIN Patients PT ON A.PatientID = PT.PatientID
                    INNER JOIN Users U ON PT.UserID = U.UserID
                    ORDER BY A.AppointmentDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Appointments.Add(new AppointmentModelData
                        {
                            AppointmentID = reader.GetInt32(0),
                            PatientName = reader.GetString(1),
                            HealthIssue = reader.GetString(2),
                            Department = reader.GetString(3),
                            AppointmentDate = reader.GetDateTime(4),
                            Status = reader.GetString(5)
                        });
                    }
                }
            }
        }
    }

    public class AppointmentModelData
    {
        public int AppointmentID { get; set; }
        public string PatientName { get; set; }
        public string HealthIssue { get; set; }
        public string Department { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
    }
}