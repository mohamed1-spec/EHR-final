using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace EHR.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly string connectionString =
            "Server=DESKTOP-3CKHFVG\\SQLEXPRESS01;Database=EHR_DB;Trusted_Connection=True;";

        [BindProperty] public string IDNumber { get; set; } = string.Empty;
        [BindProperty] public string PhoneNumber { get; set; } = string.Empty;
        [BindProperty] public string FirstName { get; set; } = string.Empty;
        [BindProperty] public string LastName { get; set; } = string.Empty;
        [BindProperty] public DateTime DateOfBirth { get; set; } = DateTime.Today;
        [BindProperty] public string Username { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;

        [BindProperty] public string UserType { get; set; } = "Patient"; // Patient / Doctor

        public string? Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Username", Username);
                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        Message = "⚠ Username already exists, please choose another one.";
                        return Page();
                    }
                }

                
                string insertUserQuery = @"
                    INSERT INTO Users
                        (IDNumber, PhoneNumber, FirstName, LastName, DateOfBirth, Username, Password, IsApproved, Role)
                    VALUES
                        (@IDNumber, @PhoneNumber, @FirstName, @LastName, @DateOfBirth, @Username, @Password, @IsApproved, @Role);
                    SELECT SCOPE_IDENTITY();";

                int newUserId;
                using (SqlCommand cmd = new SqlCommand(insertUserQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@IDNumber", IDNumber ?? string.Empty);
                    cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber ?? string.Empty);
                    cmd.Parameters.AddWithValue("@FirstName", FirstName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@LastName", LastName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    cmd.Parameters.AddWithValue("@Username", Username ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Password", Password ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Role", UserType ?? "Patient");

                  
                    bool isApproved = !UserType.Equals("Doctor", StringComparison.OrdinalIgnoreCase);
                    cmd.Parameters.AddWithValue("@IsApproved", isApproved);

                    object result = cmd.ExecuteScalar();
                    newUserId = Convert.ToInt32(result);
                }

                if (UserType.Equals("Patient", StringComparison.OrdinalIgnoreCase))
                {
                   

                    string addPatient = @"
                        INSERT INTO Patients (UserID, Age, Gender, Address, BloodType)
                        VALUES (@UserID, 0, 'Unknown', 'Not Provided', 'N/A');";

                    using (SqlCommand patientCmd = new SqlCommand(addPatient, conn))
                    {
                        patientCmd.Parameters.AddWithValue("@UserID", newUserId);
                        patientCmd.ExecuteNonQuery();
                    }
                }
                
                else if (UserType.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
                {
                    string addDoctor = @"
                        INSERT INTO Doctors (UserID)
                        VALUES (@UserID);";

                    using (SqlCommand doctorCmd = new SqlCommand(addDoctor, conn))
                    {
                        doctorCmd.Parameters.AddWithValue("@UserID", newUserId);
                        doctorCmd.ExecuteNonQuery();
                    }
                }

               
                Message = "✅ Registration successful! You can now log in.";
            }

            return Page();
        }
    }
}