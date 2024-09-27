using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using PersonalizedLibraryAPI.Models;
using PersonalizedLibraryAPI.DTOs;
using PersonalizedLibraryAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FrontEnd.Pages
{
    public class SharedBasePage : PageModel
    {
        protected readonly UserManager<AppUser> _userManager;

        public SharedBasePage(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        protected async Task<string> GetUserIdFromTokenAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var email = jwtToken.Claims.First(claim => claim.Type == "email").Value;
            var appUser = await _userManager.FindByEmailAsync(email);
            return appUser?.Id;
        }
    }
}