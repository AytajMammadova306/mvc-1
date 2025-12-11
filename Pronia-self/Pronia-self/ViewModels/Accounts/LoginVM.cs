using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace Pronia_self.ViewModels
{
    public class LoginVM
    {
        [MinLength(4)]
        [MaxLength(128)]
        public string UserNameOrEmail { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsPersistnet { get; set; }
    }
}
