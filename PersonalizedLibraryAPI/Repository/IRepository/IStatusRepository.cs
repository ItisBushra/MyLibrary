using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Models;

namespace PersonalizedLibraryAPI.Repository.IRepository
{
    public interface IStatusRepository
    {
        ICollection<Status> GetStatuses();
        Status GetStatus(int id);
        Status GetStatusByBook(int bookId);
        ICollection<Book> GetBooksFromAStatus(int statusId);
        bool StatusExists(int id);
    }
}