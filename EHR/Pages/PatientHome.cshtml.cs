using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EHR.Pages
{
    public class PatientHomeModel : PageModel
    {
        private readonly string connectionString =
            "Server=DESKTOP-3CKHFVG\\SQLEXPRESS01;Database=EHR_DB;Trusted_Connection=True;";

        public string PatientName { get; set; } = "";
        public string NotificationMessage { get; set; } = "";
        public string? LastPrescription { get; set; }
        public string? DoctorName { get; set; }
        public DateTime? PrescriptionDate { get; set; }

        public void OnGet()
        {
            int patientId = HttpContext.Session.GetInt32("PatientID") ?? 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand nameCmd = new SqlCommand(@"
                    SELECT FirstName, LastName 
                    FROM Users 
                    WHERE UserID = (SELECT UserID FROM Patients WHERE PatientID = @PID)",
                    conn))
                {
                    nameCmd.Parameters.AddWithValue("@PID", patientId);
                    using var nameReader = nameCmd.ExecuteReader();
                    if (nameReader.Read())
                    {
                        PatientName = $"{nameReader["FirstName"]} {nameReader["LastName"]}";
                    }
                    nameReader.Close();
                }

                using (SqlCommand presCmd = new SqlCommand(@"
                    SELECT TOP 1 
                        P.PrescriptionText,
                        P.DateIssued,
                        U.FirstName + ' ' + U.LastName AS DoctorName
                    FROM Prescriptions P
                    INNER JOIN Doctors D ON P.DoctorID = D.DoctorID
                    INNER JOIN Users U ON D.UserID = U.UserID
                    WHERE P.PatientID = @PID
                    ORDER BY P.DateIssued DESC",
                    conn))
                {
                    presCmd.Parameters.AddWithValue("@PID", patientId);
                    using var presReader = presCmd.ExecuteReader();
                    if (presReader.Read())
                    {
                        LastPrescription = presReader["PrescriptionText"].ToString();
                        DoctorName = presReader["DoctorName"].ToString();
                        PrescriptionDate = Convert.ToDateTime(presReader["DateIssued"]);
                    }
                    presReader.Close();
                }

                using (SqlCommand apptCmd = new SqlCommand(@"
                    SELECT TOP 1 AppointmentDate
                    FROM Appointments
                    WHERE PatientID = @PID
                      AND Status = 'Approved'
                      AND IsNotified = 0
                    ORDER BY AppointmentDate DESC",
                    conn))
                {
                    apptCmd.Parameters.AddWithValue("@PID", patientId);

                    object result = apptCmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        DateTime visit = Convert.ToDateTime(result);

                        NotificationMessage =
                            $"🟢 Your appointment has been approved for {visit:yyyy-MM-dd HH:mm}";

                        using (SqlCommand mark = new SqlCommand(@"
                            UPDATE Appointments
                            SET IsNotified = 1
                            WHERE PatientID = @PID AND Status='Approved' AND IsNotified=0",
                            conn))
                        {
                            mark.Parameters.AddWithValue("@PID", patientId);
                            mark.ExecuteNonQuery();
                        }

                        return; 
                    }
                }

                if (!string.IsNullOrEmpty(LastPrescription))
                {
                    NotificationMessage =
                        $"💊 New prescription from Dr. {DoctorName} on {PrescriptionDate:yyyy-MM-dd HH:mm}";
                }
            }
        }
    }
}