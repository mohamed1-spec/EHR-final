using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EHR.Data;    
using EHR.Models;  
using System.Linq;

namespace EHR.Pages         
{
    public class PatientProfileModel : PageModel  
    {
        private readonly EhrDbContext _context;

        public PatientProfileModel(EhrDbContext context)
        {
            _context = context;
        }

        public Patient? Patient { get; set; }

        public void OnGet(int id)
        {
            Patient = _context.Patients.AsNoTracking()
                      .FirstOrDefault(p => p.PatientId == id);
        }
    }
}