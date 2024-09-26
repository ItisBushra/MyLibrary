using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using PersonalizedLibraryAPI.Models;

namespace PersonalizedLibraryAPI.Repository.IRepository
{
    public interface ITokenServiceRepository
    {
        string CreateToken(AppUser user);
        ClaimsPrincipal ValidateToken(string token);
    }
}