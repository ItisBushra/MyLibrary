using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
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
            return _dBContext.Books
                    .Where(b=>b.Id == id).FirstOrDefault();
        }
        public Book GetBook(string name)
        {
            return _dBContext.Books
                    .Where(b=>b.Name == name).FirstOrDefault();
        }
        public ICollection<Book> GetBooks()
        {
            return _dBContext.Books
                    .OrderBy(b=>b.Id).ToList();
        }
        public bool CreateBook(int categoryId, int statusId, Book book,
                         ReadingTracking? readingTracking, Review? review)
        {
            //fetching the category
            var BookCategoryObj = _dBContext.Categories.Where(c=>c.Id == categoryId).FirstOrDefault();

            // assigning the obj
            var BookCategory = new BookCategory()
            {
                Category = BookCategoryObj,
                Book = book
            };
            
            //fetching the status
            var BookStatus = _dBContext.Statuses.Where(s=>s.Id == statusId).FirstOrDefault();
            book.StatusId = BookStatus.Id;

            //if a review is populated
            if(review !=null)
            {
                _dBContext.Reviews.Add(review); 
                book.Review = review;
            }
            //if a reading tracking is populated
            if(readingTracking !=null)
            {
                _dBContext.ReadingTrackings.Add(readingTracking);
                book.ReadingTracking = readingTracking;
            }
            _dBContext.Add(BookCategory);
            _dBContext.Add(book);
            return Save();
        }

        public bool Save()
        {
            var saved = _dBContext.SaveChanges();
            return saved > 0 ? true: false;
        }
    }
}