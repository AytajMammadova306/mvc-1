using Microsoft.AspNetCore.Identity;

namespace Pronia_self.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}
