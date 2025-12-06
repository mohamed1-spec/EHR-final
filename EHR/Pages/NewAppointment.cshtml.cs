using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EHR.Pages
{
    public class NewAppointmentModel : PageModel
    {
        private readonly string connectionString = "Server=DESKTOP-3CKHFVG\\SQLEXPRESS01;Database=EHR_DB;Trusted_Connection=True;";

        [BindProperty] public string HealthIssue { get; set; }
        [BindProperty] public string Department { get; set; }
        [BindProperty] public int Age { get; set; }
        [BindProperty] public int Weight { get; set; }
        [BindProperty] public int Height { get; set; }
        [BindProperty] public string HealthProblems { get; set; }
        [BindProperty] public string City { get; set; }
        [BindProperty] public string Neighborhood { get; set; }

        public string Message { get; set; }

        public void OnGet() { }

        public void OnPost()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string doctorQuery = "SELECT TOP 1 DoctorID FROM Doctors ORDER BY NEWID();";
          
                SqlCommand doctorCmd = new SqlCommand(doctorQuery, conn);
                var doctorResult = doctorCmd.ExecuteScalar();
                int doctorId = doctorResult != null ? Convert.ToInt32(doctorResult) : 0;

                string patientQuery = "SELECT TOP 1 PatientID FROM Patients ORDER BY PatientID DESC";
                SqlCommand patientCmd = new SqlCommand(patientQuery, conn);
                var patientResult = patientCmd.ExecuteScalar();
                int patientId = Convert.ToInt32(patientResult);

                string query = @"INSERT INTO Appointments 
                                (PatientID, DoctorID, HealthIssue, Department, Age, Weight, Height, HealthProblems, City, Neighborhood, AppointmentDate, Status)
                                VALUES (@PatientID, @DoctorID, @HealthIssue, @Department, @Age, @Weight, @Height, @HealthProblems, @City, @Neighborhood, GETDATE(), 'New')";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", patientId);
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.Parameters.AddWithValue("@HealthIssue", HealthIssue ?? "");
                cmd.Parameters.AddWithValue("@Department", Department ?? "");
                cmd.Parameters.AddWithValue("@Age", Age);
                cmd.Parameters.AddWithValue("@Weight", Weight);
                cmd.Parameters.AddWithValue("@Height", Height);
                cmd.Parameters.AddWithValue("@HealthProblems", HealthProblems ?? "");
                cmd.Parameters.AddWithValue("@City", City ?? "");
                cmd.Parameters.AddWithValue("@Neighborhood", Neighborhood ?? "");

                cmd.ExecuteNonQuery();
            }

            Message = "✅ Appointment sent to an available doctor successfully.";
        }
    }
}