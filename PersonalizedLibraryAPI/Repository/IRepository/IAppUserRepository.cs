using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Models;

namespace PersonalizedLibraryAPI.Repository.IRepository
{
    public interface IAppUserRepository
    {
        ICollection<AppUser> GetAppUsers();
        AppUser GetAppUser(string id);
        AppUser GetAppUserByBook(int bookId);
        bool AppUserExists(string id);
    }
}