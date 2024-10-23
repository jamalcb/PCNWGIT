using Microsoft.EntityFrameworkCore;
using PCNW.Models;
using System.ComponentModel.DataAnnotations;

namespace PCNW.Helpers
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        public readonly string _connectionString;
        public UniqueEmailAttribute()
        {
            _connectionString = "Server=EC2AMAZ-9Q84QA4;Database=OCPCProjectDB;Integrated Security=True;MultipleActiveResultSets=True;";

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string uniqueName = (string)value;
                var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(_connectionString).Options;
                using (ApplicationDbContext _dbContext = new ApplicationDbContext(options))
                {
                    var tblmember = _dbContext.Users.Where(m => m.Email == uniqueName).FirstOrDefault();
                    if (tblmember == null)
                    {
                        return ValidationResult.Success;
                    }
                }
            }

            return new ValidationResult("Email already exists");
        }
    }
}
