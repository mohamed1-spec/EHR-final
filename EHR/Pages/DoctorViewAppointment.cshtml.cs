using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EHR.Pages
{
    public class DoctorViewAppointmentModel : PageModel
    {
        private readonly string connectionString =
            "Server=DESKTOP-3CKHFVG\\SQLEXPRESS01;Database=EHR_DB;Trusted_Connection=True;";

        [BindProperty]
        public AppointmentData AppointmentInfo { get; set; } = new AppointmentData();

        [BindProperty]
        public string PrescriptionText { get; set; } = string.Empty;

        [BindProperty]
        public DateTime VisitDate { get; set; }

        public string? Message { get; set; }

        public void OnGet(int id)
        {
            LoadAppointment(id);

            if (TempData.ContainsKey("Success"))
                Message = TempData["Success"]?.ToString();
            else if (TempData.ContainsKey("Error"))
                Message = TempData["Error"]?.ToString();
        }

        private void LoadAppointment(int id)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string query = @"
                SELECT 
                    A.AppointmentID,
                    A.PatientID,
                    A.DoctorID,
                    A.HealthIssue,
                    A.Department,
                    A.Age,
                    A.Weight,
                    A.Height,
                    A.HealthProblems,
                    A.City,
                    A.Neighborhood,
                    A.AppointmentDate,
                    A.Status,
                    U.FirstName + ' ' + U.LastName AS PatientName
                FROM Appointments A
                INNER JOIN Patients P ON A.PatientID = P.PatientID
                INNER JOIN Users U ON P.UserID = U.UserID
                WHERE A.AppointmentID = @id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using SqlDataReader r = cmd.ExecuteReader();
            if (r.Read())
            {
                AppointmentInfo.AppointmentID = id;
                AppointmentInfo.PatientID = Convert.ToInt32(r["PatientID"]);
                AppointmentInfo.DoctorID = Convert.ToInt32(r["DoctorID"]);
                AppointmentInfo.HealthDescription = r["HealthIssue"].ToString();
                AppointmentInfo.DeptName = r["Department"].ToString();
                AppointmentInfo.Age = Convert.ToInt32(r["Age"]);
                AppointmentInfo.Weight = Convert.ToInt32(r["Weight"]);
                AppointmentInfo.Height = Convert.ToInt32(r["Height"]);
                AppointmentInfo.HealthProblems = r["HealthProblems"].ToString();
                AppointmentInfo.City = r["City"].ToString();
                AppointmentInfo.Neighborhood = r["Neighborhood"].ToString();
                AppointmentInfo.AppointmentDate = Convert.ToDateTime(r["AppointmentDate"]);
                AppointmentInfo.Status = r["Status"].ToString();
                AppointmentInfo.PatientName = r["PatientName"].ToString();
            }
        }

        public IActionResult OnPostConfirmAppointment(int id)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string updateQuery = @"
                UPDATE Appointments
                SET AppointmentDate = @VisitDate,
                    Status = 'Approved',
                    IsNotified = 0
                WHERE AppointmentID = @id";

            using SqlCommand cmd = new SqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@VisitDate", VisitDate);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            TempData["Success"] = "✅ Appointment confirmed and sent to patient.";
            return RedirectToPage("/DoctorViewAppointment", new { id });
        }

        public IActionResult OnPostSavePrescription(int id)
        {
            
            LoadAppointment(id);

            int patientId = AppointmentInfo.PatientID;
            int doctorId = AppointmentInfo.DoctorID;

            if (patientId <= 0 || doctorId <= 0)
            {
                TempData["Error"] = "⚠ Missing patient or doctor ID. Prescription not saved.";
                return RedirectToPage("/DoctorViewAppointment", new { id });
            }

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            
            string insertQuery = @"
                INSERT INTO Prescriptions (PatientID, DoctorID, PrescriptionText, DateIssued)
                VALUES (@PID, @DID, @TEXT, GETDATE())";

            using SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@PID", patientId);
            cmd.Parameters.AddWithValue("@DID", doctorId);
            cmd.Parameters.AddWithValue("@TEXT", PrescriptionText);
            cmd.ExecuteNonQuery();

            
            string updateQuery = @"
                UPDATE Appointments
                SET Status = 'Completed',
                    IsNotified = 0
                WHERE AppointmentID = @AID";

            using SqlCommand cmd2 = new SqlCommand(updateQuery, conn);
            cmd2.Parameters.AddWithValue("@AID", id);
            cmd2.ExecuteNonQuery();

            TempData["Success"] = "💊 Prescription saved and sent to patient.";
            return RedirectToPage("/DoctorViewAppointment", new { id });
        }
    }

    public class AppointmentData
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public string? HealthDescription { get; set; }
        public string? DeptName { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public string? HealthProblems { get; set; }
        public string? City { get; set; }
        public string? Neighborhood { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; }
        public string? PatientName { get; set; }
    }
}