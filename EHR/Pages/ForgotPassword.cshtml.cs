using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EHR.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty]
        public string UsernameOrId { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(UsernameOrId) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
            {
                Message = "All fields are required.";
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                Message = "Passwords do not match.";
                return Page();
            }


            Message = "Your password has been reset successfully.";
            return Page();
        }
    }
}