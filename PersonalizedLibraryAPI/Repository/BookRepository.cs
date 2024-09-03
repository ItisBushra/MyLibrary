using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Data;
using PersonalizedLibraryAPI.Models;
using PersonalizedLibraryAPI.Repository.IRepository;

namespace PersonalizedLibraryAPI.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DBContext _dBContext;
        public BookRepository(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public bool BookExists(int id)
        {
            return _dBContext.Books.Any(b=>b.Id == id);
        }

        public Book GetBook(int id)
        {
            return _dBContext.Books.Where(b=>b.Id == id).FirstOrDefault();
        }

        public Book GetBook(string name)
        {
            return _dBContext.Books.Where(b=>b.Name == name).FirstOrDefault();
        }

        public ICollection<Book> GetBooks()
        {
            return _dBContext.Books.OrderBy(b=>b.Id).ToList();
        }
    }
}