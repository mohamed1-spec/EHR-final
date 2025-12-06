using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace EHR.Pages
{
    public class LoginModel : PageModel
    {
        private string connectionString = "Server=DESKTOP-3CKHFVG\\SQLEXPRESS01;Database=EHR_DB;Trusted_Connection=True;";

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string? Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT UserID, Role, IsApproved 
                                 FROM Users 
                                 WHERE Username=@Username AND Password=@Password";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@Password", Password);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int userId = Convert.ToInt32(reader["UserID"]);
                    string role = reader["Role"].ToString();
                    bool isApproved = Convert.ToBoolean(reader["IsApproved"]);

                   
                    HttpContext.Session.SetInt32("UserID", userId);

                    reader.Close(); 

                    if (role == "Doctor")
                    {
                        if (!isApproved)
                        {
                            Message = "Your account is pending approval by admin.";
                            return Page();
                        }

                        HttpContext.Session.Remove("PatientID");

                        using (SqlCommand getDoc = new SqlCommand(
                            "SELECT DoctorID FROM Doctors WHERE UserID=@UID", conn))
                        {
                            getDoc.Parameters.AddWithValue("@UID", userId);
                            object docResult = getDoc.ExecuteScalar();

                            if (docResult != null)
                                HttpContext.Session.SetInt32("DoctorID", Convert.ToInt32(docResult));
                        }

                        return RedirectToPage("/DoctorHome");
                    }

                    if (role == "Patient")
                    {
                        HttpContext.Session.Remove("DoctorID");

                        using (SqlCommand getPatient = new SqlCommand(
                            "SELECT PatientID FROM Patients WHERE UserID=@UID", conn))
                        {
                            getPatient.Parameters.AddWithValue("@UID", userId);
                            object pidResult = getPatient.ExecuteScalar();

                            if (pidResult != null)
                            {
                                int patientId = Convert.ToInt32(pidResult);

                                HttpContext.Session.SetInt32("PatientID", patientId);
                            }
                        }

                        return RedirectToPage("/PatientHome");
                    }
                }
                else
                {
                    Message = "Invalid username or password.";
                }

                reader.Close();
                return Page();
            }
        }
    }
}