using Microsoft.EntityFrameworkCore;
using EHR.Models;

namespace EHR.Data
{
    public class EhrDbContext : DbContext
    {
        public EhrDbContext(DbContextOptions<EhrDbContext> options) : base(options) { }
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
    }
}
