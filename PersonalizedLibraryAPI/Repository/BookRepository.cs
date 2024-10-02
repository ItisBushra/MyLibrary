using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using PersonalizedLibraryAPI.Data;
using PersonalizedLibraryAPI.Models;
using PersonalizedLibraryAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

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
        public ICollection<Book> GetBooks(string id = null)
        {
            IQueryable<Book> books = _dBContext.Books.Include(s=>s.Status)
                    
                    .Include(r=>r.Review)
                    .Include(re=>re.ReadingTracking)
                    .Include(c=>c.BookCategories).ThenInclude(bc => bc.Category);

            if (!string.IsNullOrEmpty(id))
            {
                books = books.Include(u=>u.AppUser).Where(b => b.AppUser.Id == id);
            }
            return books.OrderBy(b=>b.Id).ToList();
        }
        public bool CreateBook(List<int> categoryId, int statusId, Book book,
                         ReadingTracking? readingTracking, Review? review, string userId)
        {
            //kullanıcı getirme
            var BookUser = _dBContext.AppUsers.Where(s=>s.Id == userId).FirstOrDefault();
            book.AppUserId = BookUser.Id;

            //kategoriyi getirme
            var BookCategoryObj = _dBContext.Categories.Where(c => categoryId.Contains(c.Id)).ToList();

            // objenin atanması
            var BookCategory = BookCategoryObj.Select(category => new BookCategory
            {
                Category = category,
                Book = book
            }).ToList();
            //status getirme
            var BookStatus = _dBContext.Statuses.Where(s=>s.Id == statusId).FirstOrDefault();
            book.StatusId = BookStatus.Id;

            //eğer rivew null değilse
            if(review !=null)
            {
                _dBContext.Reviews.Add(review); 
                book.Review = review;
            }
            //eğer reading tracking null değilse
            if(readingTracking !=null)
            {
                _dBContext.ReadingTrackings.Add(readingTracking);
                book.ReadingTracking = readingTracking;
            }
            _dBContext.BookCategories.AddRange(BookCategory);
            _dBContext.Add(book);
            return Save();
        }

        public bool UpdateBook(List<int> categoryId, int statusId, Book book, ReadingTracking? readingTracking, Review? review
        , string userId)
        {
            var existingBook = _dBContext.Books
                            .Include(b => b.BookCategories)
                                .ThenInclude(bc => bc.Category)
                            .Include(b => b.ReadingTracking)
                            .Include(b => b.Review)
                            .Include(b=>b.ReadingTracking)
                            .Include(b=>b.Status)//edit this in previouse pics
                            .Include(b=>b.AppUser)
                            .FirstOrDefault(b => b.Id == book.Id);

            if (existingBook == null)
            {
                return false;
            }
            existingBook.Name = book.Name;
            existingBook.WritersName = book.WritersName;
            existingBook.StatusId = statusId;
            existingBook.AppUserId = userId;

            var existingBookCategory = existingBook.BookCategories.ToList();
            foreach(var category in existingBookCategory)
            {
                _dBContext.Remove(category);
            }
            foreach(var catId in categoryId)
            {
                existingBook.BookCategories.
                    Add(new BookCategory { BookId = book.Id, CategoryId = catId});
            }
            
            var existingBookReview = existingBook.Review;
            if (existingBookCategory != null)
            {
                _dBContext.Remove(existingBookReview);
            }
            existingBook.Review = new Review {
                     BookId = book.Id,
                     Title = review.Title,
                     Text = review.Text,
                     Liked = review.Liked
                };

            var existingBookReadingTracking = existingBook.Review;
            if (existingBookReadingTracking != null)
            {
                _dBContext.Remove(existingBookReview);
            }
            existingBook.ReadingTracking = new ReadingTracking {
                     BookId = book.Id,
                     StartDate = readingTracking.StartDate,
                     EndDate = readingTracking.EndDate,
                };

            _dBContext.Update(existingBook);
            return Save();
        }

        public bool DeleteBook(Book book)
        {
            var bookToBeDeleted = _dBContext.Books.Include(b => b.BookCategories)
                            .Include(b => b.ReadingTracking)
                            .Include(b => b.Review)
                            .Include(b=>b.ReadingTracking)
                            .Include(b=>b.Review)
                            .FirstOrDefault(b => b.Id == book.Id);

            // İlgili İncelemeyi silme
            if (book.Review != null)
            {
                _dBContext.Reviews.Remove(book.Review);
            }

            // İlgili okuma takibi silme
            if (book.ReadingTracking != null)
            {
                _dBContext.ReadingTrackings.Remove(book.ReadingTracking);
            }

            // İlgili kitabın kategorisi silme
            if (book.BookCategories != null)
            {
                _dBContext.BookCategories.RemoveRange(book.BookCategories);
            }     

            _dBContext.Remove(book);
            return Save();
        }
        public bool Save()
        {
            var saved = _dBContext.SaveChanges();
            return saved > 0 ? true: false;
        }
    }
}