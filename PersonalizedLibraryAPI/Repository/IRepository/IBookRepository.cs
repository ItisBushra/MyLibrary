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
        bool CreateBook(List<int> categoryId, int statusId, Book book,
                         ReadingTracking? readingTracking, Review? review);

        bool UpdateBook(List<int> categoryId, int statusId, Book book, ReadingTracking? readingTracking, Review? review);
        bool DeleteBook(Book book);
        bool Save();
    }
}