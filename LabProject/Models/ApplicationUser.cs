using Microsoft.AspNetCore.Identity;

namespace LabProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } // Ekstra alanlar buraya eklenebilir
    }
}
