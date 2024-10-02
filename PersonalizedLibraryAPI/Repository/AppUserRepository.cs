using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Models;
using PersonalizedLibraryAPI.Data;
using Microsoft.EntityFrameworkCore; 
using System.Linq; 

using PersonalizedLibraryAPI.Repository.IRepository;

namespace PersonalizedLibraryAPI.Repository
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly DBContext _dBContext;
        public AppUserRepository(DBContext dBContext)
        {
            _dBContext = dBContext;
        }
        public bool AppUserExists(string id)
        {
           return _dBContext.AppUsers
                    .Any(s=>s.Id == id);
        }
        public AppUser GetAppUser(string id)
        {
             return _dBContext.AppUsers
             .Where(s=>s.Id == id)
             .FirstOrDefault();
        }
        public AppUser GetAppUserByBook(int bookId)
        {
             return _dBContext.Books
             .Where(b=>b.Id == bookId)
             .Select(s=>s.AppUser)
             .FirstOrDefault();
        }
        public ICollection<AppUser> GetAppUsers()
        {
            return _dBContext.AppUsers.ToList();
        }
    }
}