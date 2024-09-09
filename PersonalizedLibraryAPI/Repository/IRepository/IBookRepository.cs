using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Models;

namespace PersonalizedLibraryAPI.Repository.IRepository
{
    public interface IBookRepository
    {
        ICollection<Book> GetBooks();
        Book GetBook(int id);
        Book GetBook(string name);
        bool BookExists(int id);
        bool CreateBook(int categoryId, int statusId, Book book,
                         ReadingTracking? readingTracking, Review? review);

        bool UpdateBook(int categoryId, int statusId, Book book, ReadingTracking? readingTracking, Review? review);
        bool Save();
    }
}