using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EHR.Pages
{
    public class MyPrescriptionsModel : PageModel
    {
        private readonly string connectionString = "Server=DESKTOP-3CKHFVG\\SQLEXPRESS01;Database=EHR_DB;Trusted_Connection=True;";
        public List<PrescriptionData> Prescriptions { get; set; } = new();

        public void OnGet()
        {
            int patientId = HttpContext.Session.GetInt32("PatientID") ?? 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT P.PrescriptionText, P.DateIssued, U.FirstName + ' ' + U.LastName AS DoctorName
                                 FROM Prescriptions P
                                 INNER JOIN Doctors D ON P.DoctorID = D.DoctorID
                                 INNER JOIN Users U ON D.UserID = U.UserID
                                 WHERE P.PatientID = @PatientID
                                 ORDER BY P.DateIssued DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", patientId);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Prescriptions.Add(new PrescriptionData
                    {
                        PrescriptionText = reader["PrescriptionText"].ToString(),
                        DateIssued = Convert.ToDateTime(reader["DateIssued"]),
                        DoctorName = reader["DoctorName"].ToString()
                    });
                }
            }
        }
    }

    public class PrescriptionData
    {
        public string? DoctorName { get; set; }
        public string? PrescriptionText { get; set; }
        public DateTime DateIssued { get; set; }
    }
}