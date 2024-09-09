using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Repository.IRepository;
using PersonalizedLibraryAPI.Data;
using PersonalizedLibraryAPI.Models;
using AutoMapper;

namespace PersonalizedLibraryAPI.Repository
{
    public class ReviewRepository: IReviewRepository
    {
        private readonly DBContext _dBContext;
        public ReviewRepository(DBContext dBContext)
        {
            _dBContext = dBContext;
        }
        public bool ReviewExists(int id)
        {
            return _dBContext.Reviews.Any(r=>r.Id == id);
        }
        public ICollection<Review> GetReviews() 
        {
            return _dBContext.Reviews.OrderBy(r=>r.Id).ToList();
        }
        public Review GetReview(int id)
        {
            return _dBContext.Reviews.Where(r=>r.Id == id).FirstOrDefault();
        }
        public Review GetReviewByBook(int bookId)
        {
            return _dBContext.Books.Where(b=>b.Id == bookId).Select(r=>r.Review).FirstOrDefault();
        }
        public Book GetBookByReview(int reviewId)
        {
            return _dBContext.Reviews.Where(r=>r.Id == reviewId).Select(b=>b.Book).FirstOrDefault();
        }
        public bool CreateReview(Review review)
        {
            _dBContext.Add(review);
            return Save();
        }
        public bool UpdateReview(Review review)
        {
            _dBContext.Update(review);
            return Save();
        }
        public bool Save()
        {
            var saved = _dBContext.SaveChanges();
            return saved > 0 ? true: false;
        }   
    }
}