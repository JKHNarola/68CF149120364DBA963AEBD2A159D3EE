using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace App.BAL.Data.DTO
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(60)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(60)]
        public string LastName { get; set; }
    }
}
