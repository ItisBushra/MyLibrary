using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PersonalizedLibraryAPI.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Book>? Books { get; set; }
    }
}