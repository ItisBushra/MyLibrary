using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizedLibraryAPI.DTOs.Account
{
    public class RegisterDto
    {
        [Required (ErrorMessage = "Bu alanı zorunludur." )]
        public string? Username { get; set; }
        [Required (ErrorMessage = "Bu alanı zorunludur." )]
        [EmailAddress]
        public string? Email { get; set; }
        [Required (ErrorMessage = "Bu alanı zorunludur." )]
        public string? Password { get; set; }
    }
}