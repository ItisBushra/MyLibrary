using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizedLibraryAPI.DTOs.Account
{
    public class LoginDto
    {
        [Required (ErrorMessage = "Bu alanı zorunludur." )]
        public string UserName { get; set; }
        [Required (ErrorMessage = "Bu alanı zorunludur." )]
        public string Password { get; set; }
    }
}