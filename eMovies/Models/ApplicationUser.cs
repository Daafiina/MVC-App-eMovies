using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace eMovies.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Display(Name ="Fullname")]
        public  string FullName { get; set; }

    }
}
